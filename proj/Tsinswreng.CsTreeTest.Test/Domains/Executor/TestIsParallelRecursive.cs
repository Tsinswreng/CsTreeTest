using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Executor;

/// 本檔只測 ITestNode.IsParallelRecursive：
/// 為真時該節點及其全部遞歸子節點不參與並行，
/// 且該聲明向下繼承。
public partial class TestExecutor{
	/// 把 IsParallelRecursive 的用例掛到節點上。
	public void RegisterIsParallelRecursive(ITestNode Node){
		var R = Node.MkRegister<TestExecutor>(
			typeof(ITestNode)
			,nameof(ITestNode.IsParallelRecursive)
			,"Para:"
		).Register;
		var T = Assert.IsTrue;

		R("IsParallelRecursive 讓子樹不與外部並行", async (o) => {
			var Probe = new ConcurrencyProbe();
			var Host = new TestNode{UniqName = "Ser"};
			// 兩棵各自被標記為「不參與並行」的分支，
			// 每棵內部兩條用例。
			for(var b = 0; b < 2; b++){
				var Branch = Host.NewChild($"B{b}");
				Branch.IsParallelRecursive = true;
				for(var i = 0; i < 2; i++){
					AddCase(Branch, $"{b}_{i}", async (o2) => await Probe.Run());
				}
			}
			var S = await new TreeTestExecutor().Run(
				[Host]
				,new OptTestExecutor{IsParallel = true, MaxDegreeOfParallelism = 4}
			);
			T(S.Total == 4 && S.Passed == 4);
			// 任何時刻都只有一條用例在跑。
			T(Probe.Max == 1);
			return NIL;
		});

		R("IsParallelRecursive 向下繼承到更深層", async (o) => {
			var Probe = new ConcurrencyProbe();
			var Host = new TestNode{UniqName = "Deep"};
			// 標記在最外層，
			// 用例掛在三層之下，
			// 仍應受約束。
			var Outer = Host.NewChild("Outer");
			Outer.IsParallelRecursive = true;
			var Mid = Outer.NewChild("Mid");
			AddCase(Mid, "deep0", async (o2) => await Probe.Run());
			var Mid2 = Outer.NewChild("Mid2");
			AddCase(Mid2, "deep1", async (o2) => await Probe.Run());

			var S = await new TreeTestExecutor().Run(
				[Host]
				,new OptTestExecutor{IsParallel = true, MaxDegreeOfParallelism = 4}
			);
			T(S.Total == 2 && S.Passed == 2);
			T(Probe.Max == 1);
			return NIL;
		});
	}
}