using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

// 實現文件：只放函數實現。
public partial class TestNodeTree{
	public partial void RegisterPathString(ITestNode Node){
		var R = Node.MkRegister<TestNodeTree>(
			typeof(ITestNode)
			,nameof(ITestNodeExtn.PathString)
			,"Path:"
		).Register;

		R(nameof(PathString_NestedChain_RootToLeafSlashJoined), PathString_NestedChain_RootToLeafSlashJoined!);
	}

	public partial async Task<nil> PathString_NestedChain_RootToLeafSlashJoined(obj? O){
		var T = Assert.IsTrue;
		var A = new TestNode{UniqName = "A"};
		var B = A.NewChild("B");
		var C = B.NewChild("C");
		T(C.PathString() == "A/B/C");
		return NIL;
	}
}