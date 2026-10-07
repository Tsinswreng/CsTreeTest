using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Selection;

// 實現文件：只放函數實現。
public partial class TestSelectionCase{
	public partial void RegisterRun(ITestNode Node){
		var R = Node.MkRegister<TestSelectionCase>(
			typeof(TestSelection)
			,nameof(TestSelection.Run)
			,"Sel:"
		).Register;

		R(nameof(SelectionIsRunnableDirectly), SelectionIsRunnableDirectly!);
	}

	public partial async Task<nil> SelectionIsRunnableDirectly(obj? O){
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;
		var Sel = Mgr.Filter()
			.TesteeFn(nameof(IFakeSvc.Echo))
			.Select();
		T(Sel.Count > 0);
		var S = await Sel.Run(new OptTestExecutor{IsParallel = false});
		T(S.Total == Sel.Count);
		T(S.Failed == 0);
		return NIL;
	}
}