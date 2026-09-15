using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.TestFnRegister;

// 被測：TestFnRegister ——「每次 Register 使用最新選項」的可變語義。
// 聲明文件：只寫函數聲明（partial method）與 Doc；實現見對應 .Impl.cs。
public partial class TestTestFnRegister:ITester{
	/// 組裝本 tester：註冊 TestFnRegister 的各個分片用例。
	public partial ITestNode RegisterTestsInto(ITestNode? Node);
}