using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

/// 測建節點的各種途徑。
public partial class TestNodeTree{
	/// 把建節點的用例掛到節點上。
	public void RegisterNewChild(ITestNode Node){
		var R = Node.MkRegister<TestNodeTree>(
			typeof(ITestNode)
			,nameof(ExtnITestNode.NewChild)
			,"NewChild:"
		).Register;
		var T = Assert.IsTrue;
		var Host = new TestNode{UniqName = "Host"};

		R("不給名字時默認名非空", async (o) => {
			var C = Host.NewChild();
			T(C.UniqName.Length > 0);
			T(Host.Children.Contains(C));
			T(C.Parents.Contains(Host));
			return NIL;
		});

		R("顯式給名字", async (o) => {
			var C = Host.NewChild("Given");
			T(C.UniqName == "Given");
			return NIL;
		});

		R("NewTesterChild 記下測試類並以類名為默認名", async (o) => {
			var C = Host.NewTesterChild(typeof(TestNodeTree));
			T(C.TesterType == typeof(TestNodeTree));
			T(C.UniqName == nameof(TestNodeTree));
			return NIL;
		});
	}
}