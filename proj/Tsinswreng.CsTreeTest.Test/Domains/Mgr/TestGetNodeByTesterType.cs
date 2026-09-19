using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

/// 本檔只測 ExtnTestMgr.GetNodeByTesterType：
/// 按測試類類型取它掛的那個節點。
public partial class TestMgrCase{
	/// 把 GetNodeByTesterType 的用例掛到節點上。
	public void RegisterGetNodeByTesterType(ITestNode Node){
		var R = Node.MkRegister<TestMgrCase>(
			typeof(TestMgr)
			,nameof(ExtnTestMgr.GetNodeByTesterType)
			,"Query:"
		).Register;
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		R("按測試類找節點", async (o) => {
			var N = Mgr.GetNodeByTesterType(typeof(TestNodeTree));
			T(N is ITesterNode TN && TN.TesterType == typeof(TestNodeTree));
			return NIL;
		});
	}
}