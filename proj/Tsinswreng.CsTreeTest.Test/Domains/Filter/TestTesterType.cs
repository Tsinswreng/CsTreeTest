using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

/// 本檔只測 TestFilter.TesterType：
/// 按測試類類型篩選用例。
public partial class TestFilterCase{
	/// 把 TesterType 的用例掛到節點上。
	public void RegisterTesterType(ITestNode Node){
		var R = Node.MkRegister<TestFilterCase>(
			typeof(TestFilter)
			,nameof(TestFilter.TesterType)
			,"Cond:"
		).Register;
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		R("按測試類篩選", async (o) => {
			var Hits = Mgr.Filter().TesterType(typeof(TestNodeTree)).Apply();
			T(Hits.Count > 0);
			T(Hits.All(x => x.Data!.TesterType == typeof(TestNodeTree)));
			return NIL;
		});
	}
}