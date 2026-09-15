using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.DiEtMgr;

// 分片聲明文件：本片測試 RegisterTester 與四種查找 API；實現見 TestIndexAndLookup.Impl.cs。
public partial class TestDiEtTestMgr{
	/// 用例：按名查找 / 按 TesterType 查找 / 按被測類型與函數名查找。
	public partial void RegisterIndexAndLookup(ITestNode Node);
}