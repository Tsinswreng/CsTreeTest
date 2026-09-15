using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.DiEtMgr;

// 實現文件：只放函數實現。
public partial class TestDiEtTestMgr{
	public partial ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterIndexAndLookup(Node);
		RegisterDuplicateGuard(Node);
		RegisterSubMgr(Node);
		RegisterQueryTestCase(Node);
		return Node;
	}
}