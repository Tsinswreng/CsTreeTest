using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.MkTestFnRegister;

// 被測：ExtnITestNode.MkTestFnRegister —— 泛型重載與舊 4 參數重載的註冊行為。
// 聲明文件：只寫函數聲明（partial method）與 Doc；實現見對應 .Impl.cs。
// 分片方法（RegisterGenericMeta 等）的聲明在各分片 Xxx.cs 中。
public partial class TestMkTestFnRegister:ITester{
	/// 組裝本 tester：註冊 MkTestFnRegister 的各個分片用例。
	public partial ITestNode RegisterTestsInto(ITestNode? Node);
}