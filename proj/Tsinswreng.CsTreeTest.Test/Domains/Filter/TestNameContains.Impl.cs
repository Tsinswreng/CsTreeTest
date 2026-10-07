using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.Executor;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

// 實現文件：只放函數實現。
public partial class TestFilterCase{
	public partial void RegisterNameContains(ITestNode Node){
		var R = Node.MkRegister<TestFilterCase>(
			typeof(TestFilter)
			,nameof(TestFilter.NameContains)
			,"Cond:"
		).Register;

		R(nameof(NameContains_CaseNameFragment_ExactSingleHit), NameContains_CaseNameFragment_ExactSingleHit!);
		R(nameof(NameContains_NoMatch_ReturnsEmpty), NameContains_NoMatch_ReturnsEmpty!);
	}

	public partial async Task<nil> NameContains_CaseNameFragment_ExactSingleHit(obj? O){
		// 每條用例自己取 T 與 Mgr，不依賴註冊時的局部。
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		var All = Mgr.Filter().Apply();
		// 片段取 Executor 域那條用例的方法名；
		// 以 nameof 取值，用例改名後編譯期就會報錯，不會靜默零命中。
		var Hits = Mgr.Filter()
			.NameContains(nameof(TestExecutor.SequentialModeKeepsDeclaredOrder))
			.Apply();
		T(Hits.Count == 1);
		T(Hits[0].Data!.UniqName == "Order:" + nameof(TestExecutor.SequentialModeKeepsDeclaredOrder));
		T(All.Count > Hits.Count);
		return NIL;
	}

	public partial async Task<nil> NameContains_NoMatch_ReturnsEmpty(obj? O){
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		var Hits = Mgr.Filter().NameContains("絕不存在的用例名").Apply();
		T(Hits.Count == 0);
		return NIL;
	}
}