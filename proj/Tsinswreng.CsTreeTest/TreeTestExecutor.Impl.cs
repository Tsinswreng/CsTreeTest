using System.Collections.Concurrent;
using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public partial class TestRunFailedException{
	public partial TestRunFailedException(TestRunSummary summary)
		: base($"Test run failed. Total={summary.Total}, Passed={summary.Passed}, Failed={summary.Failed}")
	{
		Summary = summary;
	}
}

public sealed partial class TreeTestExecutor{
	public partial Task<TestRunSummary> Run(
		IList<ITestNode> Nodes,
		OptTestExecutor? Opt
	){
		ArgumentNullException.ThrowIfNull(Nodes);
		Opt ??= new OptTestExecutor();

		var startedAt = DateTimeOffset.Now;
		var workStages = CollectWorkStages(Nodes);
		var results = new ConcurrentBag<TestCaseRunResult>();
		var parallelOptions = new ParallelOptions{
			MaxDegreeOfParallelism = Opt.MaxDegreeOfParallelism <= 0
				? Environment.ProcessorCount
				: Opt.MaxDegreeOfParallelism,
		};

		return RunCore(startedAt, workStages, results, parallelOptions, Opt.Arg, Opt.IsParallel);
	}

	private static async partial Task<TestRunSummary> RunCore(
		DateTimeOffset startedAt,
		IList<IList<WorkItem>> workStages,
		ConcurrentBag<TestCaseRunResult> results,
		ParallelOptions parallelOptions,
		obj? arg,
		bool isParallel
	){
		if(isParallel){
			foreach(var stage in workStages){
				var parallelItems = stage.Where(x => !x.ShouldNotParallelize).ToList();
				var sequentialItems = stage.Where(x => x.ShouldNotParallelize).ToList();
				
				if(parallelItems.Count > 0){
					await Parallel.ForEachAsync(parallelItems, parallelOptions, async (item, _) => {
						await ExecuteItem(item, results, arg);
					});
				}
				
				foreach(var item in sequentialItems){
					await ExecuteItem(item, results, arg);
				}
			}
		}
		else{
			foreach(var stage in workStages){
				foreach(var item in stage){
					parallelOptions.CancellationToken.ThrowIfCancellationRequested();
					await ExecuteItem(item, results, arg);
				}
			}
		}

		var ordered = results.OrderBy(x => x.Order).ToList();
		var passed = ordered.Count(x => x.IsPassed);
		var failed = ordered.Count - passed;

		return new TestRunSummary{
			StartedAt = startedAt,
			EndedAt = DateTimeOffset.Now,
			Total = ordered.Count,
			Passed = passed,
			Failed = failed,
			Results = ordered,
		};
	}

	private static async partial Task ExecuteItem(
		WorkItem item,
		ConcurrentBag<TestCaseRunResult> results,
		obj? arg
	){
		var sw = System.Diagnostics.Stopwatch.StartNew();
		var isPassed = false;
		Exception? exception = null;
		try{
			await item.TestCase.FnTest(arg);
			isPassed = true;
		}catch(Exception e){
			exception = e;
		}
		results.Add(new TestCaseRunResult{
			Order = item.Order,
			NodePath = item.NodePath,
			Node = item.Node,
			TestCase = item.TestCase,
			IsPassed = isPassed,
			Exception = exception,
			Elapsed = sw.Elapsed,
		});
	}

	private static partial IList<IList<WorkItem>> CollectWorkStages(IList<ITestNode> nodes){
		var output = new List<IList<WorkItem>>();
		var order = 0;
		for(var i = 0; i < nodes.Count; i++){
			var childStages = CollectNodeStages(nodes[i], $"{i}", ref order, false);
			for(var j = 0; j < childStages.Count; j++){
				EnsureStage(output, j);
				foreach(var item in childStages[j]){
					output[j].Add(item);
				}
			}
		}
		return output;
	}

	private static partial IList<IList<WorkItem>> CollectNodeStages(
		ITestNode node,
		str path,
		ref int order,
		bool parentIsParallelRecursive
	){
		var shouldNotParallelize = parentIsParallelRecursive || node.IsParallelRecursive;

		if(node.Data is ITestCase testCase){
			var leaf = new List<IList<WorkItem>>(){
				new List<WorkItem>(){
					new WorkItem{
						Order = order++,
						NodePath = path,
						Node = node,
						TestCase = testCase,
						ShouldNotParallelize = shouldNotParallelize,
					}
				}
			};
			return leaf;
		}

		var output = new List<IList<WorkItem>>();

		// Ordered：每個子樹的 stage 依序追加成串行鏈
		if(node.Ordered){
			for(var i = 0; i < node.Children.Count; i++){
				var childStages = CollectNodeStages(node.Children[i], $"{path}/{i}", ref order, shouldNotParallelize);
				foreach(var stage in childStages){
					output.Add(stage);
				}
			}
			return output;
		}

		// 默認並行：子樹按 stage 索引合併（stage barrier）
		var mergedChildStages = new List<IList<WorkItem>>();
		for(var i = 0; i < node.Children.Count; i++){
			var childStages = CollectNodeStages(node.Children[i], $"{path}/{i}", ref order, shouldNotParallelize);
			for(var j = 0; j < childStages.Count; j++){
				EnsureStage(mergedChildStages, j);
				for(var k = 0; k < childStages[j].Count; k++){
					mergedChildStages[j].Add(childStages[j][k]);
				}
			}
		}

		for(var i = 0; i < mergedChildStages.Count; i++){
			output.Add(mergedChildStages[i]);
		}

		return output;
	}

	private static partial void EnsureStage(IList<IList<WorkItem>> stages, int index){
		while(stages.Count <= index){
			stages.Add([]);
		}
	}

	private static partial void Dfs(
		ITestNode node,
		str path,
		IList<WorkItem> output,
		ref int order
	){
		if(node.Data is ITestCase testCase){

			output.Add(new WorkItem{
				Order = order++,
				NodePath = path,
				Node = node,
				TestCase = testCase,
				ShouldNotParallelize = false,
			});
		}
		for(var i = 0; i < node.Children.Count; i++){
			Dfs(node.Children[i], $"{path}/{i}", output, ref order);
		}
	}
}

// ITestExecutor 的擴展：一行跑測試 / 跑完打印匯總（實現文件）。
public static partial class ExtnTreeTestExecutor{
	public static partial Task<TestRunSummary> RunTests(
		this ITestExecutor z
		,ITestNode Root
		,obj? Arg
		,OptTestExecutor? Opt
	){
		Opt ??= new OptTestExecutor();
		Opt.Arg = Arg;
		return z.Run([Root], Opt);
	}

	public static async partial Task<TestRunSummary> RunEtPrint(
		this ITestExecutor z
		,ITestNode Root
		,obj? Arg
		,OptTestExecutor? Opt
		,bool ThrowOnFailed
	){
		Opt ??= new OptTestExecutor();
		Opt.Arg = Arg;
		var summary = await z.Run([Root], Opt);
		WriteSummary(summary);
		if(ThrowOnFailed && summary.Failed > 0){
			throw new TestRunFailedException(summary);
		}
		return summary;
	}

	private static partial void WriteSummary(TestRunSummary summary){
		foreach(var result in summary.Results.OrderBy(x => x.Order)){
			if(result.IsPassed){
				continue;
			}
			var testerName = result.TestCase.TesterType?.Name ?? "UnknownTester";
			WriteWithColor("[FAIL]", ConsoleColor.Red);
			Console.WriteLine($" {testerName}/{result.TestCase.UniqName} ({result.Elapsed})");
			if(result.Exception is not null){
				WriteLineWithColor(result.Exception.ToString(), ConsoleColor.DarkRed);
			}
		}
		System.Console.WriteLine();
		WriteWithColor($"[{nameof(CsTreeTest)}]", ConsoleColor.Cyan);
		System.Console.WriteLine();
		Console.Write($"Total={summary.Total}");
		System.Console.WriteLine();
		WriteWithColor($"Passed={summary.Passed}", summary.Passed > 0 ? ConsoleColor.Green : Console.ForegroundColor);
		System.Console.WriteLine();
		WriteWithColor($"Failed={summary.Failed}", summary.Failed > 0 ? ConsoleColor.Red : Console.ForegroundColor);
		System.Console.WriteLine();
		Console.WriteLine($"Elapsed={summary.Elapsed}");
		System.Console.WriteLine();
	}
	
	/// 彩色輸出（帶換行）。
	private static partial void WriteWithColor(str text, ConsoleColor color){
		var oldColor = Console.ForegroundColor;
		Console.ForegroundColor = color;
		Console.Write(text);
		Console.ForegroundColor = oldColor;
	}

	/// 彩色輸出（不換行）。
	private static partial void WriteLineWithColor(str text, ConsoleColor color){
		var oldColor = Console.ForegroundColor;
		Console.ForegroundColor = color;
		Console.WriteLine(text);
		Console.ForegroundColor = oldColor;
	}
}