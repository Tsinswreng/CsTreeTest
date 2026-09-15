using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Assertions;

// 被測：Assert.IsTrue —— 斷言通過不拋、失敗拋異常。
// 聲明文件：只寫函數聲明（partial method）與 Doc；實現見對應 .Impl.cs。
public partial class TestAssert:ITester{
	/// 組裝本 tester：註冊 Assert 的各個分片用例。
	public partial ITestNode RegisterTestsInto(ITestNode? Node);
}