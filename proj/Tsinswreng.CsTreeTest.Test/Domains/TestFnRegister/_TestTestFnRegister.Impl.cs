using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.TestFnRegister;

// 實現文件：只放函數實現。
public partial class TestTestFnRegister{
	public partial ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterMutableOptions(Node);
		return Node;
	}
}