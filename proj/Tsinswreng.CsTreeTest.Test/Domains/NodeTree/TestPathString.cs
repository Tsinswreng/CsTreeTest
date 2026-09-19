using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

/// 本檔只測 ExtnITestNode.PathString：
/// 把根到當前節點這一路的名字拼成字符串。
public partial class TestNodeTree{
	/// 把 PathString 的用例掛到節點上。
	public void RegisterPathString(ITestNode Node){
		var R = Node.MkRegister<TestNodeTree>(
			typeof(ITestNode)
			,nameof(ExtnITestNode.PathString)
			,"Path:"
		).Register;
		var T = Assert.IsTrue;

		R("PathString 拼出根到葉", async (o) => {
			var A = new TestNode{UniqName = "A"};
			var B = A.NewChild("B");
			var C = B.NewChild("C");
			T(C.PathString() == "A/B/C");
			return NIL;
		});
	}
}