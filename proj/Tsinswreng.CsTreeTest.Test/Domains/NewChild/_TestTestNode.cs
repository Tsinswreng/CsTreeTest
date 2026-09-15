using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.NewChild;

// 被測：ExtnITestNode.NewChild —— 樹節點的掛接、命名與 TesterNode 行為。
// 聲明文件：只寫函數聲明（partial method）與 Doc；實現見對應 .Impl.cs。
// 分片方法（RegisterNewChild 等）的聲明在各分片 Xxx.cs 中，不在此重複。
public partial class TestTestNode:ITester{
	/// 組裝本 tester：註冊 NewChild 的各個分片用例。
	public partial ITestNode RegisterTestsInto(ITestNode? Node);
}