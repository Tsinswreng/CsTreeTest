using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

/// 本檔只測 TestFilter.NameContains：
/// 按用例名片段篩選，
/// 無命中時返回空列表。
public partial class TestFilterCase{
	/// 把 NameContains 的用例掛到節點上。
	public void RegisterNameContains(ITestNode Node){
		var R = Node.MkRegister<TestFilterCase>(
			typeof(TestFilter)
			,nameof(TestFilter.NameContains)
			,"Cond:"
		).Register;
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		R("按名稱片段篩選", async (o) => {
			var All = Mgr.Filter().Apply();
			var Hits = Mgr.Filter().NameContains("依序模式").Apply();
			T(Hits.Count == 1);
			T(Hits[0].Data!.UniqName == "Order:依序模式保持聲明次序");
			T(All.Count > Hits.Count);
			return NIL;
		});

		R("無命中時返回空列表", async (o) => {
			var Hits = Mgr.Filter().NameContains("絕不存在的用例名").Apply();
			T(Hits.Count == 0);
			return NIL;
		});
	}
}