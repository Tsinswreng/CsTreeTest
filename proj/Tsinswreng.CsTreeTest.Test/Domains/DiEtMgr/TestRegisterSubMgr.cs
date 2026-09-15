using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.DiEtMgr;

// 分片聲明文件：本片測試 RegisterSubMgr 掛載；實現見 TestRegisterSubMgr.Impl.cs。
public partial class TestDiEtTestMgr{
	/// 用例：子 Mgr 的測試掛到父樹、裝載點下可見 tester 節點。
	public partial void RegisterSubMgr(ITestNode Node);
}