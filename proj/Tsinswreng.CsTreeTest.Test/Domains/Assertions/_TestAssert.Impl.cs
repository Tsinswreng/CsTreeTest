using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Assertions;

// 實現文件：只放函數實現。
public partial class TestAssert{
	public partial ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterIsTrue(Node);
		return Node;
	}
}
