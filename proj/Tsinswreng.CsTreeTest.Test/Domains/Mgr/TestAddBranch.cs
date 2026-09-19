using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

/// 本檔只測 TestMgr.AddBranch：
/// 在根下建領域分組節點，同名時復用同一個。
public partial class TestMgrCase{
	/// 把 AddBranch 的用例掛到節點上。
	public void RegisterAddBranch(ITestNode Node){
		var R = Node.MkRegister<TestMgrCase>(
			typeof(TestMgr)
			,nameof(TestMgr.AddBranch)
			,"Branch:"
		).Register;
		var T = Assert.IsTrue;

		R("AddBranch 建分組並去重", async (o) => {
			var Sub = new OneTesterMgr();
			Sub.Init();
			var B1 = Sub.AddBranch("Area");
			var B2 = Sub.AddBranch("Area");
			// 同名分組必須復用同一個節點，
			// 否則唯一名會撞。
			T(ReferenceEquals(B1, B2));
			return NIL;
		});
	}
}