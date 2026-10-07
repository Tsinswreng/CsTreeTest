using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 實現文件：只放函數實現。
public partial class TestMgrCase{
	public partial void RegisterWalkCases(ITestNode Node){
		var R = Node.MkRegister<TestMgrCase>(
			typeof(TestMgr)
			,nameof(ExtnTestMgr.WalkCases)
			,"Walk:"
		).Register;

		R(nameof(WalkCases_OnlyCaseNodes_ReturnsLeafOnly), WalkCases_OnlyCaseNodes_ReturnsLeafOnly!);
	}

	public partial async Task<nil> WalkCases_OnlyCaseNodes_ReturnsLeafOnly(obj? O){
		var T = Assert.IsTrue;
		var A = new TestNode{UniqName = "A"};
		var Leaf = A.NewChild("Leaf");
		Leaf.Data = new TestCase{UniqName = "Leaf"};
		var WithData = A.WalkCases().ToList();
		T(WithData.Count == 1);
		T(WithData[0].UniqName == "Leaf");
		return NIL;
	}
}