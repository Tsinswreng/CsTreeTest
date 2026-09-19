using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

/// 本檔只測 ExtnITestNode.PathFromRoot：
/// 返回從根到當前節點的名字列表。
public partial class TestNodeTree{
	/// 把 PathFromRoot 的用例掛到節點上。
	public void RegisterPathFromRoot(ITestNode Node){
		var R = Node.MkRegister<TestNodeTree>(
			typeof(ITestNode)
			,nameof(ExtnITestNode.PathFromRoot)
			,"Path:"
		).Register;
		var T = Assert.IsTrue;

		R("PathFromRoot 返回名字列表", async (o) => {
			var A = new TestNode{UniqName = "A"};
			var B = A.NewChild("B");
			var C = B.NewChild("C");
			T(string.Join("/", C.PathFromRoot()) == "A/B/C");
			return NIL;
		});
	}
}