using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Index;

// 實現文件：只放函數實現。
public partial class TestIndexCase{
	public partial ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterAdd(Node);
		return Node;
	}
}