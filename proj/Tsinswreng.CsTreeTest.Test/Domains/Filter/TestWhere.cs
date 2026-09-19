using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

/// 本檔只測 TestFilter.Where：
/// 自訂謂詞篩選。
public partial class TestFilterCase{
	/// 把 Where 的用例掛到節點上。
	public void RegisterWhere(ITestNode Node){
		var R = Node.MkRegister<TestFilterCase>(
			typeof(TestFilter)
			,nameof(TestFilter.Where)
			,"Cond:"
		).Register;
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		R("Where 自訂條件", async (o) => {
			var Hits = Mgr.Filter()
				.Where(x => x.UniqName.Contains("失敗"))
				.Apply();
			T(Hits.Count > 0);
			T(Hits.All(x => x.UniqName.Contains("失敗")));
			return NIL;
		});
	}
}