using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

namespace Tsinswreng.CsTreeTest.Test.Domains.Selection;

// 實現文件：只放函數實現。
public partial class TestSelectionCase{
	public partial void RegisterSelect(ITestNode Node){
		var R = Node.MkRegister<TestSelectionCase>(
			typeof(TestSelection)
			,nameof(TestFilter.Select)
			,"Sel:"
		).Register;

		R(nameof(SelectYieldsCountableSelection), SelectYieldsCountableSelection!);
	}

	public partial async Task<nil> SelectYieldsCountableSelection(obj? O){
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;
		var Sel = Mgr.Filter()
			.TesterType(typeof(TestNodeTree))
			.Select();
		T(Sel.Count > 0);
		T(Sel.Cases().All(x => x.TesterType == typeof(TestNodeTree)));
		return NIL;
	}
}