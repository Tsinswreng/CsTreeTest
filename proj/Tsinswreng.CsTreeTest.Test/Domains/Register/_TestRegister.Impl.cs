using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Register;

// 實現文件：只放函數實現。
public partial class TestRegister{
	public partial ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterMkRegister(Node);
		return Node;
	}
}