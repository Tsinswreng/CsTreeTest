using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Executor;

// 分片聲明文件：本片測試執行統計與異常捕獲；實現見 TestSummary.Impl.cs。
public partial class TestTreeTestExecutor{
	/// 用例：Total/Passed/Failed 統計、失敗用例的異常被捕獲進 Result。
	public partial void RegisterSummary(ITestNode Node);
}