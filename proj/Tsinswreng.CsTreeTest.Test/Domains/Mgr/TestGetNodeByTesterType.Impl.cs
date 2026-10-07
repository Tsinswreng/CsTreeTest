using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 實現文件：只放函數實現。
public partial class TestMgrCase{
	public partial void RegisterGetNodeByTesterType(ITestNode Node){
		var R = Node.MkRegister<TestMgrCase>(
			typeof(TestMgr)
			,nameof(ExtnTestMgr.GetNodeByTesterType)
			,"Query:"
		).Register;

		R(nameof(GetNodeByTesterType_RegisteredTester_ReturnsItsNode), GetNodeByTesterType_RegisteredTester_ReturnsItsNode!);
	}

	public partial async Task<nil> GetNodeByTesterType_RegisteredTester_ReturnsItsNode(obj? O){
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;
		var N = Mgr.GetNodeByTesterType(typeof(TestNodeTree));
		T(N is ITesterNode TN && TN.TesterType == typeof(TestNodeTree));
		return NIL;
	}
}