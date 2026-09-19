using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.Register;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

/// 本檔只測 TestMgr.Init：
/// 重複初始化要拒絕，
/// 重複註冊同一測試類也要在掛樹時被擋下。
public partial class TestMgrCase{
	/// 把初始化防護的用例掛到節點上。
	public void RegisterInit(ITestNode Node){
		var R = Node.MkRegister<TestMgrCase>(
			typeof(TestMgr)
			,nameof(TestMgr.Init)
			,"Init:"
		).Register;
		var T = Assert.IsTrue;

		R("管理器拒絕重複註冊同一測試類", async (o) => {
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
		});

		R("初始化前不可重複 Init", async (o) => {
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
		});
	}
}