using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.NewChild;

public partial class TestTestNode{
	public partial void RegisterNewChild(ITestNode Node){
		var R = Node.MkTestFnRegister<TestTestNode>().Register;
		var T = Assert.IsTrue;

		R("NewChild 掛子節點並回填 Parents", async (o)=>{
			var Root = new TestNode();
			var Child = Root.NewChild("c1");
			T(Root.Children.Count == 1 && Root.Children[0] == Child);
			T(Child.Parents.Count == 1 && Child.Parents[0] == Root);
			return null;
		});

		R("NewChild 預設 UniqName 非空", async (o)=>{
			var Root = new TestNode();
			var Child = Root.NewChild();
			T(!String.IsNullOrWhiteSpace(Child.UniqName));
			return null;
		});

		R("NewChild 顯式 UniqName 生效", async (o)=>{
			var Root = new TestNode();
			var Child = Root.NewChild("Named");
			T(Child.UniqName == "Named");
			return null;
		});

		R("NewChild<T> 產生帶 TesterType 的 TesterNode", async (o)=>{
			var Root = new TestNode();
			ITestNode Child = Root.NewChild<TestTestNode>();
			T(Child is ITesterNode TN && TN.TesterType == typeof(TestTestNode));
			return null;
		});
	}
}