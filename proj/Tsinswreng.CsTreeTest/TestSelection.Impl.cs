using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public partial class TestSelection{
	public partial TestSelection(ITestMgr Mgr, IList<ITestNode> Nodes){
		ArgumentNullException.ThrowIfNull(Mgr);
		ArgumentNullException.ThrowIfNull(Nodes);
		this.Mgr = Mgr;
		this.Nodes = Nodes;
	}

	public partial i32 Count => Nodes.Count;

	public partial void PrintHits(){
		Console.WriteLine($"命中共 {Count} 條用例：");
		foreach(var Node in Nodes){
			Console.WriteLine($"  {Node.PathString()}");
		}
	}

	public partial IEnumerable<ITestCase> Cases(){
		foreach(var Node in Nodes){
			if(Node.Data is ITestCase Case){
				yield return Case;
			}
		}
	}

	public partial ITestNode MkDetachedTree(){
		// step 1：造一個脫離原樹的根。
		var Root = new TestNode{UniqName = "SelectedCases"};

		// step 2：逐條用例掛成一級子節點，保持 `Nodes` 的次序。
		foreach(var Node in Nodes){
			if(Node.Data is not ITestCase Case){
				continue;
			}
			// 用例本身是純數據，
			// 但 TestCase 帶着可變的列表欄位，
			// 因此複製一份再掛，
			// 避免臨時樹與原樹共享同一個用例物件。
			var Copy = new TestCase{
				UniqName = Case.UniqName,
				TesterType = Case.TesterType,
				TesteeTypes = [.. Case.TesteeTypes],
				TesteeFnNames = [.. Case.TesteeFnNames],
				FnTest = Case.FnTest,
			};
			Root.Children.Add(new TestNode{
				UniqName = Case.UniqName,
				Data = Copy,
				Parents = [Root],
			});
		}
		return Root;
	}

	public partial Task<TestRunSummary> Run(OptTestExecutor? Opt, obj? Arg){
		var Executor = new TreeTestExecutor();
		return Executor.Run([MkDetachedTree()], MakeOpt(Opt, Arg));
	}

	public partial Task<TestRunSummary> RunEtPrint(OptTestExecutor? Opt, obj? Arg){
		var Executor = new TreeTestExecutor();
		return Executor.RunEtPrint([MkDetachedTree()], MakeOpt(Opt, Arg));
	}

	private static partial OptTestExecutor MakeOpt(OptTestExecutor? Opt, obj? Arg){
		// 默認執行器選項是並行的，
		// 選集要顯式關掉。
		Opt ??= new OptTestExecutor{IsParallel = false};
		// 顯式給的參數不覆蓋調用方已寫進選項的那個。
		Opt.Arg ??= Arg;
		return Opt;
	}
}
