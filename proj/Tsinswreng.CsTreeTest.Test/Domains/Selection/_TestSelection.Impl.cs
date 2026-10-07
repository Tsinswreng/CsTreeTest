using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Selection;

// 實現文件：只放函數實現。
public partial class TestSelectionCase{
	public partial ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterSelect(Node);
		RegisterMkDetachedTree(Node);
		RegisterRun(Node);
		return Node;
	}
}