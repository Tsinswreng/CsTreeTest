using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.Mgr;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

// 實現文件：只放函數實現。
public partial class TestFilterCase{
	public partial void RegisterUnder(ITestNode Node){
		var R = Node.MkRegister<TestFilterCase>(
			typeof(TestFilter)
			,nameof(TestMgrExtn.Filter)
			,"Cond:"
		).Register;

		R(nameof(Under_NarrowScope_OnlySubtreeHits), Under_NarrowScope_OnlySubtreeHits!);
		R(nameof(Under_DomainBranch_ThenTesterType_Exact), Under_DomainBranch_ThenTesterType_Exact!);
	}

	public partial async Task<nil> Under_NarrowScope_OnlySubtreeHits(obj? O){
		// 每條用例自己取 T 與 Mgr，不依賴註冊時的局部。
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		var Branch = Mgr.GetNodeByName("NodeTree");
		var Hits = Mgr.Filter(Branch).Apply();
		T(Hits.Count > 0);
		T(Hits.All(x => x.PathString().StartsWith("Tests/NodeTree/")));
		return NIL;
	}

	public partial async Task<nil> Under_DomainBranch_ThenTesterType_Exact(obj? O){
		// 每條用例自己取 T 與 Mgr，不依賴註冊時的局部。
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		var Branch = Mgr.GetNodeByName("Mgr");
		var Hits = Mgr.Filter(Branch).TesterType(typeof(TestMgrCase)).Apply();
		T(Hits.Count > 0);
		T(Hits.All(x => x.Data!.TesterType == typeof(TestMgrCase)));
		return NIL;
	}
}