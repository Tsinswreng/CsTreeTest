using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

/// 本檔只測 TestFilter.TesteeType：
/// 按被測類型篩選用例。
public partial class TestFilterCase{
	/// 把 TesteeType 的用例掛到節點上。
	public void RegisterTesteeType(ITestNode Node){
		var R = Node.MkRegister<TestFilterCase>(
			typeof(TestFilter)
			,nameof(TestFilter.TesteeType)
			,"Cond:"
		).Register;
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		R("按被測類型篩選", async (o) => {
			var Hits = Mgr.Filter().TesteeType(typeof(IFakeSvc)).Apply();
			T(Hits.Count > 0);
			T(Hits.All(x => x.Data!.TesteeTypes.Contains(typeof(IFakeSvc))));
			return NIL;
		});
	}
}