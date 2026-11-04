using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 實現文件：只放函數實現。
public partial class TestMgrCase{
	public partial void RegisterWalk(ITestNode Node){
		var R = Node.MkRegister<TestMgrCase>(
			typeof(TestMgr)
			,nameof(TestMgrExtn.Walk)
			,"Walk:"
		).Register;

		R(nameof(Walk_PreOrder_IncludesSelf), Walk_PreOrder_IncludesSelf!);
	}

	public partial async Task<nil> Walk_PreOrder_IncludesSelf(obj? O){
		var T = Assert.IsTrue;
		var A = new TestNode{UniqName = "A"};
		var B = A.NewChild("B");
		var C = A.NewChild("C");
		B.NewChild("D");
		var Names = A.Walk().Select(x => x.UniqName).ToList();
		T(string.Join(",", Names) == "A,B,D,C");
		return NIL;
	}
}