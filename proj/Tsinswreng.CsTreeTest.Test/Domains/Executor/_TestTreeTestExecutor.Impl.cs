using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Executor;

// 實現文件：只放函數實現。
public partial class TestTreeTestExecutor{
	public partial ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterSummary(Node);
		RegisterOrderAndArg(Node);
		return Node;
	}

	/// 手搓葉節點（Data 為 TestCase）；供各分片用例構造獨立小樹。
	internal static ITestNode MkLeaf(str Name, FnTest Fn){
		return new TestNode{
			UniqName = Name,
			Data = new TestCase{ UniqName = Name, FnTest = Fn },
		};
	}
}