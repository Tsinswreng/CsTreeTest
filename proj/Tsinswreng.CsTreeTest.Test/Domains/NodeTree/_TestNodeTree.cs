using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

/// 測樹節點自身的形狀。
/// 本類是主檔：
/// 只負責把各被測函數的用例檔串起來，
/// 自己不寫用例。
public partial class TestNodeTree : ITester{
	public ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterNewChild(Node);
		RegisterPathString(Node);
		RegisterPathFromRoot(Node);
		return Node;
	}
}