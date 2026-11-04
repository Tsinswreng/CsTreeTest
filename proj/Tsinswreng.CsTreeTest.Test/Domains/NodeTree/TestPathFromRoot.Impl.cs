using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

// 實現文件：只放函數實現。
public partial class TestNodeTree{
	public partial void RegisterPathFromRoot(ITestNode Node){
		var R = Node.MkRegister<TestNodeTree>(
			typeof(ITestNode)
			,nameof(ITestNodeExtn.PathFromRoot)
			,"Path:"
		).Register;

		R(nameof(PathFromRoot_NestedChain_RootToLeafNames), PathFromRoot_NestedChain_RootToLeafNames!);
	}

	public partial async Task<nil> PathFromRoot_NestedChain_RootToLeafNames(obj? O){
		var T = Assert.IsTrue;
		var A = new TestNode{UniqName = "A"};
		var B = A.NewChild("B");
		var C = B.NewChild("C");
		T(string.Join("/", C.PathFromRoot()) == "A/B/C");
		return NIL;
	}
}