using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.NewChild;

// 實現文件：只放函數實現。
public partial class TestTestNode{
	public partial ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterNewChild(Node);
		return Node;
	}
}