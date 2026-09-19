using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

/// 本檔只測 ExtnTestMgr.GetNodesByTesteeType：
/// 按被測類型取相關用例節點。
public partial class TestMgrCase{
	/// 把 GetNodesByTesteeType 的用例掛到節點上。
	public void RegisterGetNodesByTesteeType(ITestNode Node){
		var R = Node.MkRegister<TestMgrCase>(
			typeof(TestMgr)
			,nameof(ExtnTestMgr.GetNodesByTesteeType)
			,"Query:"
		).Register;
		var T = Assert.IsTrue;
		var Mgr = CsTreeTestTestMgr.Inst;

		R("按被測類型找節點", async (o) => {
			var Nodes = Mgr.GetNodesByTesteeType(typeof(IFakeSvc));
			T(Nodes.Count > 0);
			T(Nodes.All(x => x.Data!.TesteeTypes.Contains(typeof(IFakeSvc))));
			return NIL;
		});
	}
}