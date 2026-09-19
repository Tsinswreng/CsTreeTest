using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

namespace Tsinswreng.CsTreeTest.Test.Domains.Selection;

/// 本檔只測 TestSelection.Select：
/// 由篩選器造出選集，
/// 選集數量與用例清單要和條件一致。
public partial class TestSelectionCase{
	/// 把 Select 的用例掛到節點上。
	public void RegisterSelect(ITestNode Node){
		var R = Node.MkRegister<TestSelectionCase>(
			typeof(TestSelection)
			,nameof(TestFilter.Select)
			,"Sel:"
		).Register;
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		R("Select 得到選集且可數", async (o) => {
			var Sel = Mgr.Filter()
				.TesterType(typeof(TestNodeTree))
				.Select();
			T(Sel.Count > 0);
			T(Sel.Cases().All(x => x.TesterType == typeof(TestNodeTree)));
			return NIL;
		});
	}
}