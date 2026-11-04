using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 實現文件：只放函數實現。
public partial class TestMgrCase{
	public partial void RegisterGetNodesByTesteeType(ITestNode Node){
		var R = Node.MkRegister<TestMgrCase>(
			typeof(TestMgr)
			,nameof(TestMgrExtn.GetNodesByTesteeType)
			,"Query:"
		).Register;

		R(nameof(GetNodesByTesteeType_FakeSvc_AllNodesMatchTesteeType), GetNodesByTesteeType_FakeSvc_AllNodesMatchTesteeType!);
	}

	public partial async Task<nil> GetNodesByTesteeType_FakeSvc_AllNodesMatchTesteeType(obj? O){
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;
		var Nodes = Mgr.GetNodesByTesteeType(typeof(IFakeSvc));
		T(Nodes.Count > 0);
		T(Nodes.All(x => x.Data!.TesteeTypes.Contains(typeof(IFakeSvc))));
		return NIL;
	}
}