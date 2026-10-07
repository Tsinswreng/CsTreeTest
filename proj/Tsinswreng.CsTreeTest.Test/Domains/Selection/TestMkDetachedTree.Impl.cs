using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

namespace Tsinswreng.CsTreeTest.Test.Domains.Selection;

// 實現文件：只放函數實現。
public partial class TestSelectionCase{
	public partial void RegisterMkDetachedTree(ITestNode Node){
		var R = Node.MkRegister<TestSelectionCase>(
			typeof(TestSelection)
			,nameof(TestSelection.MkDetachedTree)
			,"Sel:"
		).Register;

		R(nameof(DetachedTreeKeepsOriginalOrder), DetachedTreeKeepsOriginalOrder!);
	}

	public partial async Task<nil> DetachedTreeKeepsOriginalOrder(obj? O){
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;
		var Sel = Mgr.Filter()
			.TesterType(typeof(TestNodeTree))
			.Select();
		var Detached = Sel.MkDetachedTree();
		T(Detached.UniqName == "SelectedCases");
		var Original = Mgr.Filter().TesterType(typeof(TestNodeTree)).Apply();
		T(Detached.Children.Count == Original.Count);
		for(var i = 0; i < Original.Count; i++){
			T(Detached.Children[i].Data!.UniqName == Original[i].Data!.UniqName);
		}
		return NIL;
	}
}