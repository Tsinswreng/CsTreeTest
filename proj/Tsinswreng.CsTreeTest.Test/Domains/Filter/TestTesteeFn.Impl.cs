using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

// 實現文件：只放函數實現。
public partial class TestFilterCase{
	public partial void RegisterTesteeFn(ITestNode Node){
		var R = Node.MkRegister<TestFilterCase>(
			typeof(TestFilter)
			,nameof(TestFilter.TesteeFn)
			,"Cond:"
		).Register;

		R(nameof(TesteeFn_ByName_FiltersAllMatching), TesteeFn_ByName_FiltersAllMatching!);
	}

	public partial async Task<nil> TesteeFn_ByName_FiltersAllMatching(obj? O){
		// 每條用例自己取 T 與 Mgr，不依賴註冊時的局部。
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		var Hits = Mgr.Filter().TesteeFn(nameof(IFakeSvc.Echo)).Apply();
		T(Hits.Count > 0);
		T(Hits.All(x => x.Data!.TesteeFnNames.Contains(nameof(IFakeSvc.Echo))));
		return NIL;
	}
}