using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

namespace Tsinswreng.CsTreeTest.Test.Domains.Selection;

/// 本檔只測 TestSelection.MkDetachedTree：
/// 把選中的用例掛成一棵獨立的扁平臨時樹，
/// 次序與篩選結果一致。
public partial class TestSelectionCase{
	/// 把 MkDetachedTree 的用例掛到節點上。
	public void RegisterMkDetachedTree(ITestNode Node){
		var R = Node.MkRegister<TestSelectionCase>(
			typeof(TestSelection)
			,nameof(TestSelection.MkDetachedTree)
			,"Sel:"
		).Register;
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		R("選集的臨時樹保持原次序", async (o) => {
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
		});
	}
}