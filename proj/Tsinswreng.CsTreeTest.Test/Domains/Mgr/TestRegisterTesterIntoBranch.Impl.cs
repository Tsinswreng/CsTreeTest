using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.Assertions;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 實現文件：只放函數實現。
public partial class TestMgrCase{
	public partial void RegisterRegisterTesterIntoBranch(ITestNode Node){
		var R = Node.MkRegister<TestMgrCase>(
			typeof(TestMgr)
			,nameof(TestMgr.RegisterTesterIntoBranch)
			,"Branch:"
		).Register;

		R(nameof(RegisterTesterIntoBranch_TesterNode_MountedUnderBranch), RegisterTesterIntoBranch_TesterNode_MountedUnderBranch!);
	}

	public partial async Task<nil> RegisterTesterIntoBranch_TesterNode_MountedUnderBranch(obj? O){
		var T = Assert.IsTrue;
		var Sub = new BranchMgr();
		Sub.Init();
		var N = Sub.GetNodeByTesterType(typeof(TestAssert));
		T(N.Parents.Count == 1);
		T(N.Parents[0].UniqName == "Asserts");
		T(!ReferenceEquals(N.Parents[0], Sub.Root));
		return NIL;
	}
}