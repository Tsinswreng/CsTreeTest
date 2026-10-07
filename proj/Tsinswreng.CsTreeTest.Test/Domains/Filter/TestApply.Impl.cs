using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.NodeTree;
using Tsinswreng.CsTreeTest.Test.Domains.Register;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

// 實現文件：只放函數實現。
public partial class TestFilterCase{
	public partial void RegisterApply(ITestNode Node){
		var R = Node.MkRegister<TestFilterCase>(
			typeof(TestFilter)
			,nameof(TestFilter.Apply)
			,"Cond:"
		).Register;

		R(nameof(Apply_TesterTypeAndTesteeType_AndIntersect_Exact), Apply_TesterTypeAndTesteeType_AndIntersect_Exact!);
	}

	public partial async Task<nil> Apply_TesterTypeAndTesteeType_AndIntersect_Exact(obj? O){
		// 每條用例自己取 T 與 Mgr，不依賴註冊時的局部。
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

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
	}
}