using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Executor;

/// 測執行器。
/// 本領域的用例彼此獨立，
/// 但為了結果次序穩定，
/// 把本節點設為依序執行，
/// 順便演示 Ordered 屬性的用法。
/// 本類是主檔：
/// 只負責把各被測函數的用例檔串起來，
/// 自己不寫用例。
public partial class TestExecutor : ITester{
	public ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		Node.Ordered = true;
		RegisterRun(Node);
		RegisterOrdered(Node);
		RegisterIsParallelRecursive(Node);
		return Node;
	}
}