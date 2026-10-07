using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 實現文件：只放函數實現。
public partial class TestMgrCase{
	public partial void RegisterRegisterSubMgr(ITestNode Node){
		var R = Node.MkRegister<TestMgrCase>(
			typeof(TestMgr)
			,nameof(TestMgr.RegisterSubMgr)
			,"SubMgr:"
		).Register;

		R(nameof(RegisterSubMgr_SubMgr_MergesIndex), RegisterSubMgr_SubMgr_MergesIndex!);
		R(nameof(RegisterSubMgr_SubMgrRootOrdered_PreservesOrderCfg), RegisterSubMgr_SubMgrRootOrdered_PreservesOrderCfg!);
	}

	public partial async Task<nil> RegisterSubMgr_SubMgr_MergesIndex(obj? O){
		var T = Assert.IsTrue;
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
	}

	public partial async Task<nil> RegisterSubMgr_SubMgrRootOrdered_PreservesOrderCfg(obj? O){
		var T = Assert.IsTrue;
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
	}
}