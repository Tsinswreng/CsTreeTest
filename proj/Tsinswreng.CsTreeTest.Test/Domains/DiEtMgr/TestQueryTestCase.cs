using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.DiEtMgr;

// 分片聲明文件：本片測試「查詢到的節點能取回具體用例（TestCase）」；實現見 TestQueryTestCase.Impl.cs。
public partial class TestDiEtTestMgr{
	/// 用例：索引命中的是用例葉子節點，Data 可取回 TestCase 及其 metadata。
	public partial void RegisterQueryTestCase(ITestNode Node);
}