using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

/// 測篩選器 TestFilter 的各個條件、
/// 以及按範圍篩選。
/// 選對象是本工程真正初始化過的那個管理器，
/// 篩選只讀索引，不影響並行執行。
/// 本類是主檔：
/// 只負責把各被測函數的用例檔串起來，
/// 自己不寫用例。
public partial class TestFilterCase : ITester{
	public ITestNode RegisterTestsInto(ITestNode? Node){
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