using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 實現文件：只放函數實現。
public partial class TestMgrCase{
	public partial void RegisterAddBranch(ITestNode Node){
		var R = Node.MkRegister<TestMgrCase>(
			typeof(TestMgr)
			,nameof(TestMgr.AddBranch)
			,"Branch:"
		).Register;

		R(nameof(AddBranch_SameNameTwice_ReusesSameNode), AddBranch_SameNameTwice_ReusesSameNode!);
	}

	public partial async Task<nil> AddBranch_SameNameTwice_ReusesSameNode(obj? O){
		var T = Assert.IsTrue;
		var Sub = new OneTesterMgr();
		Sub.Init();
		var B1 = Sub.AddBranch("Area");
		var B2 = Sub.AddBranch("Area");
		// 同名分組必須復用同一個節點，
		// 否則唯一名會撞。
		T(ReferenceEquals(B1, B2));
		return NIL;
	}
}