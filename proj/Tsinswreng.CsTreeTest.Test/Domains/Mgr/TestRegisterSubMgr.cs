using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

/// 本檔只測 TestMgr.RegisterSubMgr：
/// 把下級管理器的整棵子樹收編上來，
/// 並把它的索引併進本管理器的索引。
public partial class TestMgrCase{
	/// 把 RegisterSubMgr 的用例掛到節點上。
	public void RegisterRegisterSubMgr(ITestNode Node){
		var R = Node.MkRegister<TestMgrCase>(
			typeof(TestMgr)
			,nameof(TestMgr.RegisterSubMgr)
			,"SubMgr:"
		).Register;
		var T = Assert.IsTrue;

		R("RegisterSubMgr 併入索引", async (o) => {
			var Parent = new ParentMgr();
			var Sub = new OneTesterMgr();
			Parent.RegisterSubMgr(Sub);
			Parent.Init();

			// 上層能按被測函數名、被測類型找到下級用例。
			T(Parent.GetNodesByTesteeFnName(nameof(IFakeSvc.Echo)).Count > 0);
			T(Parent.GetNodesByTesteeType(typeof(IFakeSvc)).Count > 0);
			// 掛上來的節點以子管理器類型名命名，且在樹上、按名可找到。
			var Mounted = Parent.Root.Children.FirstOrDefault(c => c.UniqName == Sub.GetType().Name);
			T(Mounted is not null);
			T(ReferenceEquals(Parent.GetNodeByName(Sub.GetType().Name), Mounted));
			return NIL;
		});

		R("RegisterSubMgr 保留子管理器的順序設定", async (o) => {
			var Parent = new ParentMgr();
			var Sub = new OneTesterMgr();
			Sub.Root.Ordered = true;
			Sub.Root.IsParallelRecursive = true;
			Parent.RegisterSubMgr(Sub);
			Parent.Init();

			var Mounted = Parent.Root.Children.Single(x => x.UniqName == Sub.GetType().Name);
			T(Mounted.Ordered);
			T(Mounted.IsParallelRecursive);
			return NIL;
		});
	}
}