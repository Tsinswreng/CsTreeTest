using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.Register;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 實現文件：只放函數實現。
public partial class TestMgrCase{
	public partial void RegisterInit(ITestNode Node){
		var R = Node.MkRegister<TestMgrCase>(
			typeof(TestMgr)
			,nameof(TestMgr.Init)
			,"Init:"
		).Register;

		R(nameof(Init_DupTesterType_Throws), Init_DupTesterType_Throws!);
		R(nameof(Init_Twice_Throws), Init_Twice_Throws!);
	}

	public partial async Task<nil> Init_DupTesterType_Throws(obj? O){
		var T = Assert.IsTrue;
		var Sub = new OneTesterMgr();
		// SetupTree 會註冊一次 TestRegister，
		// 這裡再註冊一次，
		// 掛樹時第二個同型節點會迎頭撞上重複檢查。
		Sub.RegisterTester<TestRegister>();
		var Thrown = false;
		try{
			Sub.Init();
		}catch{
			Thrown = true;
		}
		T(Thrown);
		return NIL;
	}

	public partial async Task<nil> Init_Twice_Throws(obj? O){
		var T = Assert.IsTrue;
		var Sub = new OneTesterMgr();
		Sub.Init();
		var Thrown = false;
		try{
			Sub.Init();
		}catch{
			Thrown = true;
		}
		T(Thrown);
		return NIL;
	}
}