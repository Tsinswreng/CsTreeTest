using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

/// 本檔只測 ExtnTestMgr.GetNodeByName：
/// 按唯一名取節點，取不到就拋 KeyNotFound。
public partial class TestMgrCase{
	/// 把 GetNodeByName 的用例掛到節點上。
	public void RegisterGetNodeByName(ITestNode Node){
		var R = Node.MkRegister<TestMgrCase>(
			typeof(TestMgr)
			,nameof(ExtnTestMgr.GetNodeByName)
			,"Query:"
		).Register;
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		R("按名字找到根", async (o) => {
			var Root = Mgr.GetNodeByName("Tests");
			T(ReferenceEquals(Root, Mgr.Root));
			return NIL;
		});

		R("找不到時拋 KeyNotFound", async (o) => {
			var Thrown = false;
			try{
				Mgr.GetNodeByName("絕對不存在的名字");
			}catch(KeyNotFoundException){
				Thrown = true;
			}
			T(Thrown);
			return NIL;
		});
	}
}