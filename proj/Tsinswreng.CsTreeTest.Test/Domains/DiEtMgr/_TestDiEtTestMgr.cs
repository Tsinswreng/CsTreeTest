using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.DiEtMgr;

// 被測：ExtnDiEtTestMgr —— RegisterTester / RegisterSubMgr / 各查找 API 與重複保護。
// 聲明文件：只寫函數聲明（partial method）與 Doc；實現見對應 .Impl.cs。
// 假 fixture 類型（IFakeSvc/DummyTester/LocalDiMgr）見 Fixtures.cs（接口+實現類模式，不分離）。
// 分片方法（RegisterIndexAndLookup 等）的聲明在各分片 Xxx.cs 中。
public partial class TestDiEtTestMgr:ITester{
	/// 組裝本 tester：註冊 DiEtTestMgr 的各個分片用例。
	public partial ITestNode RegisterTestsInto(ITestNode? Node);
}