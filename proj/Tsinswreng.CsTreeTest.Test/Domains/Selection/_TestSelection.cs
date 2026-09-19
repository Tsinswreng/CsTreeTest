using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Selection;

/// 測選集 TestSelection：
/// 篩選的結果、由結果建臨時樹、以及直接執行選集。
/// 本類是主檔：
/// 只負責把各被測函數的用例檔串起來，
/// 自己不寫用例。
public partial class TestSelectionCase : ITester{
	public ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterSelect(Node);
		RegisterMkDetachedTree(Node);
		RegisterRun(Node);
		return Node;
	}
}