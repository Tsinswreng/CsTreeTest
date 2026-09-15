using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.DiEtMgr;

// 分片聲明文件：本片測試重複註冊保護；實現見 TestDuplicateGuard.Impl.cs。
public partial class TestDiEtTestMgr{
	/// 用例：同 tester 類型註冊兩次，InitSvc 拋 InvalidOperationException。
	public partial void RegisterDuplicateGuard(ITestNode Node);
}