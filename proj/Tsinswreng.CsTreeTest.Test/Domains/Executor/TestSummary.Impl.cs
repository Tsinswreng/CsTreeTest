using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Executor;

public partial class TestTreeTestExecutor{
	public partial void RegisterSummary(ITestNode Node){
		var R = Node.MkTestFnRegister<TestTreeTestExecutor>().Register;
		var T = Assert.IsTrue;

		R("統計通過/失敗並捕獲異常", async (o)=>{
			var Root = new TestNode();
			Root.Children.Add(MkLeaf("Pass1", async o2 => { Assert.IsTrue(true); return null; }));
			Root.Children.Add(MkLeaf("Fail1", async o2 => { Assert.IsTrue(false, "boom"); return null; }));
			Root.Children.Add(MkLeaf("Pass2", async o2 => { Assert.IsTrue(1 == 1); return null; }));

			var S = await new TreeTestExecutor().RunTests(Root, Opt: new OptTestExecutor{ IsParallel = false });
			T(S.Total == 3 && S.Passed == 2 && S.Failed == 1);
			var Fail = S.Results.FirstOrDefault(r => !r.IsPassed);
			T(Fail is not null);
			T(Fail!.Exception is not null && Fail.Exception.Message == "boom");
			return null;
		});
	}
}