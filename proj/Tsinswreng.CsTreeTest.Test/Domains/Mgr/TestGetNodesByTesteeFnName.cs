using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

/// 本檔只測 ExtnTestMgr.GetNodesByTesteeFnName：
/// 按被測函數名取相關用例節點。
public partial class TestMgrCase{
	/// 把 GetNodesByTesteeFnName 的用例掛到節點上。
	public void RegisterGetNodesByTesteeFnName(ITestNode Node){
		var R = Node.MkRegister<TestMgrCase>(
			typeof(TestMgr)
			,nameof(ExtnTestMgr.GetNodesByTesteeFnName)
			,"Query:"
		).Register;
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		R("按被測函數名找節點", async (o) => {
			var Nodes = Mgr.GetNodesByTesteeFnName(nameof(IFakeSvc.Echo));
			T(Nodes.Count > 0);
			T(Nodes.All(x => x.Data!.TesteeFnNames.Contains(nameof(IFakeSvc.Echo))));
			return NIL;
		});
	}
}