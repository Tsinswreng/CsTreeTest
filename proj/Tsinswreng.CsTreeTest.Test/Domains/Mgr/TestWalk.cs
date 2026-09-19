using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

/// 本檔只測 ExtnTestMgr.Walk：
/// 先序遍歷整棵子樹，結果含自身。
public partial class TestMgrCase{
	/// 把 Walk 的用例掛到節點上。
	public void RegisterWalk(ITestNode Node){
		var R = Node.MkRegister<TestMgrCase>(
			typeof(TestMgr)
			,nameof(ExtnTestMgr.Walk)
			,"Walk:"
		).Register;
		var T = Assert.IsTrue;

		R("Walk 先序遍歷含自身", async (o) => {
			var A = new TestNode{UniqName = "A"};
			var B = A.NewChild("B");
			var C = A.NewChild("C");
			B.NewChild("D");
			var Names = A.Walk().Select(x => x.UniqName).ToList();
			T(string.Join(",", Names) == "A,B,D,C");
			return NIL;
		});
	}
}