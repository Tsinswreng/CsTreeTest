using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

// 實現文件：只放函數實現。
public partial class TestFilterCase{
	public partial ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterTesterType(Node);
		RegisterTesteeType(Node);
		RegisterTesteeFn(Node);
		RegisterNameContains(Node);
		RegisterWhere(Node);
		RegisterApply(Node);
		RegisterUnder(Node);
		return Node;
	}
}