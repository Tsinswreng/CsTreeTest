using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.NodeTree;
using Tsinswreng.CsTreeTest.Test.Domains.Register;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

/// 本檔只測 TestFilter.Apply：
/// 多個條件之間是「同時滿足」的關係，
/// 交叉後無交集時返回空。
public partial class TestFilterCase{
	/// 把 Apply 的用例掛到節點上。
	public void RegisterApply(ITestNode Node){
		var R = Node.MkRegister<TestFilterCase>(
			typeof(TestFilter)
			,nameof(TestFilter.Apply)
			,"Cond:"
		).Register;
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		R("條件是同時滿足", async (o) => {
			// 測判斷：既要測 IFakeSvc、又要是 TestRegister 這個測試類的。
			var Hits = Mgr.Filter()
				.TesterType(typeof(TestRegister))
				.TesteeType(typeof(IFakeSvc))
				.Apply();
			T(Hits.Count > 0);
			T(Hits.All(x =>
				x.Data!.TesterType == typeof(TestRegister)
				&& x.Data!.TesteeTypes.Contains(typeof(IFakeSvc))
			));
			// TestNodeTree 的用例不測 IFakeSvc，交叉後應為空。
			var Cross = Mgr.Filter()
				.TesterType(typeof(TestNodeTree))
				.TesteeType(typeof(IFakeSvc))
				.Apply();
			T(Cross.Count == 0);
			return NIL;
		});
	}
}