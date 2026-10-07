using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

// 實現文件：只放函數實現。
public partial class TestFilterCase{
	public partial void RegisterTesteeType(ITestNode Node){
		var R = Node.MkRegister<TestFilterCase>(
			typeof(TestFilter)
			,nameof(TestFilter.TesteeType)
			,"Cond:"
		).Register;

		R(nameof(TesteeType_ByType_FiltersAllMatching), TesteeType_ByType_FiltersAllMatching!);
	}

	public partial async Task<nil> TesteeType_ByType_FiltersAllMatching(obj? O){
		// 每條用例自己取 T 與 Mgr，不依賴註冊時的局部。
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		var Hits = Mgr.Filter().TesteeType(typeof(IFakeSvc)).Apply();
		T(Hits.Count > 0);
		T(Hits.All(x => x.Data!.TesteeTypes.Contains(typeof(IFakeSvc))));
		return NIL;
	}
}