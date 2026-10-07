using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.Executor;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

// 實現文件：只放函數實現。
public partial class TestFilterCase{
	public partial void RegisterWhere(ITestNode Node){
		var R = Node.MkRegister<TestFilterCase>(
			typeof(TestFilter)
			,nameof(TestFilter.Where)
			,"Cond:"
		).Register;

		R(nameof(Where_CustomPredicate_CaseNameFragment_Filters), Where_CustomPredicate_CaseNameFragment_Filters!);
	}

	public partial async Task<nil> Where_CustomPredicate_CaseNameFragment_Filters(obj? O){
		// 每條用例自己取 T 與 Mgr，不依賴註冊時的局部。
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		// 片段取 Executor 域那條用例的方法名；
		// 以 nameof 取值，用例改名後編譯期就會報錯，不會靜默零命中。
		var Hits = Mgr.Filter()
			.Where(x => x.UniqName.Contains(nameof(TestExecutor.FailedCaseIsRecordedAndDoesNotStopOthers)))
			.Apply();
		T(Hits.Count > 0);
		T(Hits.All(x => x.UniqName.Contains(nameof(TestExecutor.FailedCaseIsRecordedAndDoesNotStopOthers))));
		return NIL;
	}
}