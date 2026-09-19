using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Selection;

/// 本檔只測 TestSelection.Run：
/// 選集可直接執行，
/// 執行結果的總數等於選中用例數。
public partial class TestSelectionCase{
	/// 把 Run 的用例掛到節點上。
	public void RegisterRun(ITestNode Node){
		var R = Node.MkRegister<TestSelectionCase>(
			typeof(TestSelection)
			,nameof(TestSelection.Run)
			,"Sel:"
		).Register;
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		R("選集可直接執行", async (o) => {
			var Sel = Mgr.Filter()
				.TesteeFn(nameof(IFakeSvc.Echo))
				.Select();
			T(Sel.Count > 0);
			var S = await Sel.Run(new OptTestExecutor{IsParallel = false});
			T(S.Total == Sel.Count);
			T(S.Failed == 0);
			return NIL;
		});
	}
}