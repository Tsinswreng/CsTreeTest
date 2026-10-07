using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

// 實現文件：只放函數實現。
public partial class TestNodeTree{
	public partial ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterNewChild(Node);
		RegisterPathString(Node);
		RegisterPathFromRoot(Node);
		return Node;
	}
}