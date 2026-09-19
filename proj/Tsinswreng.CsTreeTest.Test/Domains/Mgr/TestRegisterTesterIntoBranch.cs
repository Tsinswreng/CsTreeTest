using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.Assertions;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

/// 本檔只測 TestMgr.RegisterTesterIntoBranch：
/// 把測試類掛到指定領域分組之下，
/// 而不是直接掛在根下。
public partial class TestMgrCase{
	/// 把 RegisterTesterIntoBranch 的用例掛到節點上。
	public void RegisterRegisterTesterIntoBranch(ITestNode Node){
		var R = Node.MkRegister<TestMgrCase>(
			typeof(TestMgr)
			,nameof(TestMgr.RegisterTesterIntoBranch)
			,"Branch:"
		).Register;
		var T = Assert.IsTrue;

		R("RegisterTesterIntoBranch 把測試類歸入分組", async (o) => {
			var Sub = new BranchMgr();
			Sub.Init();
			var N = Sub.GetNodeByTesterType(typeof(TestAssert));
			T(N.Parents.Count == 1);
			T(N.Parents[0].UniqName == "Asserts");
			T(!ReferenceEquals(N.Parents[0], Sub.Root));
			return NIL;
		});
	}
}