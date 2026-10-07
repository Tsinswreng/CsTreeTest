using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Executor;

// 實現文件：只放函數實現。
public partial class TestExecutor{
	public partial ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		Node.Ordered = true;
		RegisterRun(Node);
		RegisterOrdered(Node);
		RegisterIsParallelRecursive(Node);
		return Node;
	}
}