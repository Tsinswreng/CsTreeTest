using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.Mgr;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

/// 本檔只測 ExtnTestMgr.Filter(範圍)：
/// 給一個範圍節點，
/// 篩選只在那棵子樹內進行。
public partial class TestFilterCase{
	/// 把按範圍篩選的用例掛到節點上。
	public void RegisterUnder(ITestNode Node){
		var R = Node.MkRegister<TestFilterCase>(
			typeof(TestFilter)
			,nameof(ExtnTestMgr.Filter)
			,"Cond:"
		).Register;
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		R("Under 收窄範圍", async (o) => {
			var Branch = Mgr.GetNodeByName("NodeTree");
			var Hits = Mgr.Filter(Branch).Apply();
			T(Hits.Count > 0);
			T(Hits.All(x => x.PathString().StartsWith("Tests/NodeTree/")));
			return NIL;
		});

		R("領域分組範圍下可按測試類再篩", async (o) => {
			var Branch = Mgr.GetNodeByName("Mgr");
			var Hits = Mgr.Filter(Branch).TesterType(typeof(TestMgrCase)).Apply();
			T(Hits.Count > 0);
			T(Hits.All(x => x.Data!.TesterType == typeof(TestMgrCase)));
			return NIL;
		});
	}
}