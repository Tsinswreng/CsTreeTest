using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 實現文件：只放函數實現。
public partial class TestMgrCase{
	public partial void RegisterGetNodesByTesteeFnName(ITestNode Node){
		var R = Node.MkRegister<TestMgrCase>(
			typeof(TestMgr)
			,nameof(ExtnTestMgr.GetNodesByTesteeFnName)
			,"Query:"
		).Register;

		R(nameof(GetNodesByTesteeFnName_FakeSvcEcho_AllNodesMatchTesteeFnName), GetNodesByTesteeFnName_FakeSvcEcho_AllNodesMatchTesteeFnName!);
	}

	public partial async Task<nil> GetNodesByTesteeFnName_FakeSvcEcho_AllNodesMatchTesteeFnName(obj? O){
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;
		var Nodes = Mgr.GetNodesByTesteeFnName(nameof(IFakeSvc.Echo));
		T(Nodes.Count > 0);
		T(Nodes.All(x => x.Data!.TesteeFnNames.Contains(nameof(IFakeSvc.Echo))));
		return NIL;
	}
}