using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

/// 本檔只測 ExtnTestMgr.WalkCases：
/// 先序遍歷，但只取帶用例的節點。
public partial class TestMgrCase{
	/// 把 WalkCases 的用例掛到節點上。
	public void RegisterWalkCases(ITestNode Node){
		var R = Node.MkRegister<TestMgrCase>(
			typeof(TestMgr)
			,nameof(ExtnTestMgr.WalkCases)
			,"Walk:"
		).Register;
		var T = Assert.IsTrue;

		R("WalkCases 只取帶用例的節點", async (o) => {
			var A = new TestNode{UniqName = "A"};
			var Leaf = A.NewChild("Leaf");
			Leaf.Data = new TestCase{UniqName = "Leaf"};
			var WithData = A.WalkCases().ToList();
			T(WithData.Count == 1);
			T(WithData[0].UniqName == "Leaf");
			return NIL;
		});
	}
}