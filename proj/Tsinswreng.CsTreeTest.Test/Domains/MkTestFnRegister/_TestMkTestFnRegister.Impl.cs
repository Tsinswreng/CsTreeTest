using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.MkTestFnRegister;

// 實現文件：只放函數實現。
public partial class TestMkTestFnRegister{
	public partial ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterGenericMeta(Node);
		RegisterDefaults(Node);
		RegisterLegacy(Node);
		return Node;
	}
}