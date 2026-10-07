using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.Assertions;
using Tsinswreng.CsTreeTest.Test.Domains.Register;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 實現文件：只放函數實現。
public partial class TestMgrCase{
	private sealed partial class OneTesterMgr{
		protected override partial void SetupTree(){
			RegisterTester<TestRegister>();
		}
	}

	private sealed partial class BranchMgr{
		protected override partial void SetupTree(){
			RegisterTesterIntoBranch<TestAssert>("Asserts");
		}
	}

	private sealed partial class ParentMgr{
		protected override partial void SetupTree(){
		}
	}
}