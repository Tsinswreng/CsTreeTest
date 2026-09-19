using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Index;

/// 測索引 TestIndex 本身。
/// 本類是主檔：
/// 只負責把各被測函數的用例檔串起來，
/// 自己不寫用例。
public partial class TestIndexCase : ITester{
	public ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterAdd(Node);
		return Node;
	}
}