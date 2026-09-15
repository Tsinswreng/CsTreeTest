using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Executor;

// 被測：TreeTestExecutor 與 RunTests 擴展 —— 統計、有序執行、Arg 傳遞。
// 聲明文件：只寫函數聲明（partial method）與 Doc；實現見對應 .Impl.cs。
// 分片方法（RegisterSummary 等）的聲明在各分片 Xxx.cs 中；helper（MkLeaf）見 _TestTreeTestExecutor.Impl.cs。
public partial class TestTreeTestExecutor:ITester{
	/// 組裝本 tester：註冊 TreeTestExecutor 的各個分片用例。
	public partial ITestNode RegisterTestsInto(ITestNode? Node);
}