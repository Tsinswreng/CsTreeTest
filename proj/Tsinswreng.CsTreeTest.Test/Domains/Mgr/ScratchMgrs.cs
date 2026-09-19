using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.Assertions;
using Tsinswreng.CsTreeTest.Test.Domains.Register;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

/// 本領域各用例要用的臨時管理器。
///
/// 全局那個管理器已經初始化過
/// （且執行期間並行跑用例，改它的索引不安全），
/// 所以每個要動樹結構或索引的用例
/// 都在自己新建的臨時管理器上操作。
public partial class TestMgrCase{
	/// 帶一個測試類的管理器。
	private sealed class OneTesterMgr : TestMgr{
		protected override void SetupTree(){
			RegisterTester<TestRegister>();
		}
	}

	/// 把測試類註冊到領域分組下的管理器。
	private sealed class BranchMgr : TestMgr{
		protected override void SetupTree(){
			RegisterTesterIntoBranch<TestAssert>("Asserts");
		}
	}

	/// 空的父管理器，用來收編子管理器。
	private sealed class ParentMgr : TestMgr{
		protected override void SetupTree(){
		}
	}
}