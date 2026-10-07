using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 實現文件：只放函數實現。
public partial class TestMgrCase{
	public partial void RegisterGetNodeByName(ITestNode Node){
		var R = Node.MkRegister<TestMgrCase>(
			typeof(TestMgr)
			,nameof(ExtnTestMgr.GetNodeByName)
			,"Query:"
		).Register;

		R(nameof(GetNodeByName_RootName_ReturnsRoot), GetNodeByName_RootName_ReturnsRoot!);
		R(nameof(GetNodeByName_UnknownName_ThrowsKeyNotFound), GetNodeByName_UnknownName_ThrowsKeyNotFound!);
	}

	public partial async Task<nil> GetNodeByName_RootName_ReturnsRoot(obj? O){
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;
		var Root = Mgr.GetNodeByName("Tests");
		T(ReferenceEquals(Root, Mgr.Root));
		return NIL;
	}

	public partial async Task<nil> GetNodeByName_UnknownName_ThrowsKeyNotFound(obj? O){
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;
		var Thrown = false;
		try{
			Mgr.GetNodeByName("絕對不存在的名字");
		}catch(KeyNotFoundException){
			Thrown = true;
		}
		T(Thrown);
		return NIL;
	}
}