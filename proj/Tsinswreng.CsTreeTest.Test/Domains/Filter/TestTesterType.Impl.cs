using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

// 實現文件：只放函數實現。
public partial class TestFilterCase{
	public partial void RegisterTesterType(ITestNode Node){
		var R = Node.MkRegister<TestFilterCase>(
			typeof(TestFilter)
			,nameof(TestFilter.TesterType)
			,"Cond:"
		).Register;

		R(nameof(TesterType_ByTesterType_FiltersAllMatching), TesterType_ByTesterType_FiltersAllMatching!);
	}

	public partial async Task<nil> TesterType_ByTesterType_FiltersAllMatching(obj? O){
		// 每條用例自己取 T 與 Mgr，不依賴註冊時的局部。
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		var Hits = Mgr.Filter().TesterType(typeof(TestNodeTree)).Apply();
		T(Hits.Count > 0);
		T(Hits.All(x => x.Data!.TesterType == typeof(TestNodeTree)));
		return NIL;
	}
}