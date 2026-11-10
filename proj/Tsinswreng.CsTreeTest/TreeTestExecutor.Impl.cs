using System.Collections.Concurrent;

using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public sealed partial class TreeTestExecutor{
	public partial Task<TestRunSummary> Run(
		IList<ITestNode> Nodes,
		OptTestExecutor? Opt
	){
		// 參數邊界：節點列表不能為空引用；選項為空則用默認值。
		ArgumentNullException.ThrowIfNull(Nodes);
		Opt ??= new OptTestExecutor();

		// step 1: 先記開始時間，再收集；
		// 收集本身的耗時也算在本輪總耗時裏。
		var StartedAt = DateTimeOffset.Now;
		var WorkStages = CollectWorkStages(Nodes);
		// 結果集用 ConcurrentBag：多條用例會並行寫入，普通 List 不安全。
		var Results = new ConcurrentBag<TestCaseRunResult>();
		// step 2: 非正的並行度退回到處理器數，
		// 避免把非法值交給 Parallel.ForEachAsync。
		var ParallelOptions = new ParallelOptions{
			MaxDegreeOfParallelism = Opt.MaxDegreeOfParallelism <= 0
				? Environment.ProcessorCount
				: Opt.MaxDegreeOfParallelism,
		};

		// step 3: 按階段跑，跑完匯總返回。
		return RunCore(StartedAt, WorkStages, Results, ParallelOptions, Opt.Arg, Opt.IsParallel);
	}

	private static async partial Task<TestRunSummary> RunCore(
		DateTimeOffset StartedAt,
		IList<IList<WorkItem>> WorkStages,
		ConcurrentBag<TestCaseRunResult> Results,
		ParallelOptions ParallelOptions,
		obj? Arg,
		bool IsParallel
	){
		if(IsParallel){
			foreach(var Stage in WorkStages){
				// 同一階段分兩批：
				// 先並行跑可並行項，
				// 再依序跑不參與並行項。
				var ParallelItems = Stage.Where(x => !x.ShouldNotParallelize).ToList();
				var SequentialItems = Stage.Where(x => x.ShouldNotParallelize).ToList();

				if(ParallelItems.Count > 0){
					await Parallel.ForEachAsync(ParallelItems, ParallelOptions, async (Item, _) => {
						await ExecuteItem(Item, Results, Arg);
					});
				}

				foreach(var Item in SequentialItems){
					await ExecuteItem(Item, Results, Arg);
				}
				// 本階段到此全部跑完，才進下一階段——這就是跨階段屏障。
			}
		}
		else{
			// 不並行：所有階段的所有項按收集順序單線程跑，
			// 順序即樹的先序。
			foreach(var Stage in WorkStages){
				foreach(var Item in Stage){
					ParallelOptions.CancellationToken.ThrowIfCancellationRequested();
					await ExecuteItem(Item, Results, Arg);
				}
			}
		}

		// 按收集時分配的 Order 排序，
		// 使結果順序與樹的先序一致，
		// 不受並行完成先後影響。
		var Ordered = Results.OrderBy(x => x.Order).ToList();
		var Passed = Ordered.Count(x => x.IsPassed);
		var Failed = Ordered.Count - Passed;

		return new TestRunSummary{
			StartedAt = StartedAt,
			EndedAt = DateTimeOffset.Now,
			Total = Ordered.Count,
			Passed = Passed,
			Failed = Failed,
			Results = Ordered,
		};
	}

	private static async partial Task ExecuteItem(
		WorkItem Item,
		ConcurrentBag<TestCaseRunResult> Results,
		obj? Arg
	){
		// step 1: 只計用例函數體自身的耗時，
		// 不含收集與匯總的時間。
		var Sw = System.Diagnostics.Stopwatch.StartNew();
		var IsPassed = false;
		Exception? Caught = null;
		// step 2: 跑用例。
		// 用例拋出的異常只收下、不外拋，
		// 以免中斷其餘用例。
		try{
			await Item.TestCase.FnTest(Arg);
			IsPassed = true;
		}catch(Exception E){
			Caught = E;
		}
		// step 3: 成功與失敗共用同一條收尾路徑，
		// 保證每條跑過的用例都有結果。
		Results.Add(new TestCaseRunResult{
			Order = Item.Order,
			NodePath = Item.NodePath,
			Node = Item.Node,
			TestCase = Item.TestCase,
			IsPassed = IsPassed,
			Exception = Caught,
			Elapsed = Sw.Elapsed,
		});
	}

	private static partial IList<IList<WorkItem>> CollectWorkStages(IList<ITestNode> Nodes){
		var Output = new List<IList<WorkItem>>();
		var Order = 0;
		// step 1: 逐個根節點攤平成階段。
		for(var i = 0; i < Nodes.Count; i++){
			var ChildStages = CollectNodeStages(Nodes[i], Nodes[i].PathString(), ref Order, false);
			// step 2: 該根的階段按序號併進總的階段列表；
			// 序號還不存在時由 EnsureStage 補出空階段。
			for(var j = 0; j < ChildStages.Count; j++){
				EnsureStage(Output, j);
				foreach(var Item in ChildStages[j]){
					Output[j].Add(Item);
				}
			}
		}
		return Output;
	}

	private static partial IList<IList<WorkItem>> CollectNodeStages(
		ITestNode Node,
		str Path,
		ref int Order,
		bool ParentNonParallelizable
	){
		// 不並行標記：繼承下來的與本節點自己的取或，
		// 算出的結果再往下傳（向下繼承）。
		var ShouldNotParallelize = ParentNonParallelizable || Node.NonParallelizable;

		// 情形一：葉子節點（帶用例）——
		// 本子樹只有一個階段、一項。
		if(Node.Data is ITestCase TestCase){
			return [
				[
					new WorkItem{
						Order = Order++,
						NodePath = Path,
						Node = Node,
						TestCase = TestCase,
						ShouldNotParallelize = ShouldNotParallelize,
					}
				]
			];
		}

		var Output = new List<IList<WorkItem>>();

		// 情形二：Ordered 為真，直接子節點依序執行——
		// 把各子節點的階段首尾相接成串行鏈，
		// 前面的子節點全部跑完才輪到後面的。
		if(Node.Ordered){
			for(var i = 0; i < Node.Children.Count; i++){
				var ChildStages = CollectNodeStages(
					Node.Children[i]
					,$"{Path}/{Node.Children[i].UniqName}"
					,ref Order
					,ShouldNotParallelize
				);
				foreach(var Stage in ChildStages){
					Output.Add(Stage);
				}
			}
			return Output;
		}

		// 情形三：各子節點按階段序號合併——
		// 同層的項落進同一階段，因此可以並行。
		var MergedChildStages = new List<IList<WorkItem>>();
		for(var i = 0; i < Node.Children.Count; i++){
			var ChildStages = CollectNodeStages(
				Node.Children[i]
				,$"{Path}/{Node.Children[i].UniqName}"
				,ref Order
				,ShouldNotParallelize
			);
			for(var j = 0; j < ChildStages.Count; j++){
				EnsureStage(MergedChildStages, j);
				foreach(var Item in ChildStages[j]){
					MergedChildStages[j].Add(Item);
				}
			}
		}

		foreach(var Stage in MergedChildStages){
			Output.Add(Stage);
		}
		return Output;
	}

	private static partial void EnsureStage(IList<IList<WorkItem>> Stages, int Index){
		while(Stages.Count <= Index){
			Stages.Add([]);
		}
	}
}
