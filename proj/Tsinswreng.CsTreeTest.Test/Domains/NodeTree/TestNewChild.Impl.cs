using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

// 實現文件：只放函數實現。
public partial class TestNodeTree{
	public partial void RegisterNewChild(ITestNode Node){
		var R = Node.MkRegister<TestNodeTree>(
			typeof(ITestNode)
			,nameof(ITestNodeExtn.NewChild)
			,"NewChild:"
		).Register;

		R(nameof(NewChild_NoName_DefaultUniqNameNonEmpty), NewChild_NoName_DefaultUniqNameNonEmpty!);
		R(nameof(NewChild_ExplicitName_UniqNameAsGiven), NewChild_ExplicitName_UniqNameAsGiven!);
		R(nameof(NewTesterChild_ExplicitTesterType_TypeRecordedAndNameIsClassName), NewTesterChild_ExplicitTesterType_TypeRecordedAndNameIsClassName!);
	}

	public partial async Task<nil> NewChild_NoName_DefaultUniqNameNonEmpty(obj? O){
		var T = Assert.IsTrue;
		var Host = new TestNode{UniqName = "Host"};
		var C = Host.NewChild();
		T(C.UniqName.Length > 0);
		T(Host.Children.Contains(C));
		T(C.Parents.Contains(Host));
		return NIL;
	}

	public partial async Task<nil> NewChild_ExplicitName_UniqNameAsGiven(obj? O){
		var T = Assert.IsTrue;
		var Host = new TestNode{UniqName = "Host"};
		var C = Host.NewChild("Given");
		T(C.UniqName == "Given");
		return NIL;
	}

	public partial async Task<nil> NewTesterChild_ExplicitTesterType_TypeRecordedAndNameIsClassName(obj? O){
		var T = Assert.IsTrue;
		var Host = new TestNode{UniqName = "Host"};
		var C = Host.NewTesterChild(typeof(TestNodeTree));
		T(C.TesterType == typeof(TestNodeTree));
		T(C.UniqName == nameof(TestNodeTree));
		return NIL;
	}
}