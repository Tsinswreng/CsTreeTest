using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

/// 本檔只測 TestFilter.TesteeFn：
/// 按被測函數名篩選用例。
public partial class TestFilterCase{
	/// 把 TesteeFn 的用例掛到節點上。
	public void RegisterTesteeFn(ITestNode Node){
		var R = Node.MkRegister<TestFilterCase>(
			typeof(TestFilter)
			,nameof(TestFilter.TesteeFn)
			,"Cond:"
		).Register;
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		R("按被測函數名篩選", async (o) => {
			var Hits = Mgr.Filter().TesteeFn(nameof(IFakeSvc.Echo)).Apply();
			T(Hits.Count > 0);
			T(Hits.All(x => x.Data!.TesteeFnNames.Contains(nameof(IFakeSvc.Echo))));
			return NIL;
		});
	}
}