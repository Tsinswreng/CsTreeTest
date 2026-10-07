using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Register;

// 實現文件：只放函數實現。
public partial class TestRegister{
	public partial void RegisterMkRegister(ITestNode Node){
		// 本領域測的就是「元數據會帶進用例」，
		// 因此本檔自己就用這個機制註冊用例。
		var R = Node.MkRegister<TestRegister>(
			typeof(IFakeSvc)
			,nameof(IFakeSvc.Echo)
			,"Meta:"
		).Register;

		R(nameof(MkRegister_SvcFromGlobalDi_Usable), MkRegister_SvcFromGlobalDi_Usable!);
		R(nameof(MkRegister_WithMetadata_MetadataCopiedIntoCase), MkRegister_WithMetadata_MetadataCopiedIntoCase!);
		R(nameof(MkRegister_NoMetadata_TesteeListsEmpty), MkRegister_NoMetadata_TesteeListsEmpty!);
	}

	public partial async Task<nil> MkRegister_SvcFromGlobalDi_Usable(obj? O){
		var T = Assert.IsTrue;
		T(_Svc.Echo(11) == 11);
		return NIL;
	}

	public partial async Task<nil> MkRegister_WithMetadata_MetadataCopiedIntoCase(obj? O){
		var T = Assert.IsTrue;
		var Host = new TestNode{UniqName = "MetaHost"};
		var Reg = Host.MkRegister<TestRegister>(typeof(IFakeSvc), nameof(IFakeSvc.Echo), "Echo:");
		Reg.Register("樣例", async (o2) => NIL);

		var CaseNode = Host.Children.Single(x => x.UniqName == "Echo:樣例");
		var Case = CaseNode.Data!;
		T(CaseNode.UniqName == "Echo:樣例");
		T(Case.TesterType == typeof(TestRegister));
		T(Case.TesteeTypes.Count == 1 && Case.TesteeTypes[0] == typeof(IFakeSvc));
		T(Case.TesteeFnNames.Count == 1 && Case.TesteeFnNames[0] == nameof(IFakeSvc.Echo));
		return NIL;
	}

	public partial async Task<nil> MkRegister_NoMetadata_TesteeListsEmpty(obj? O){
		var T = Assert.IsTrue;
		var Host = new TestNode{UniqName = "EmptyHost"};
		var Reg = Host.MkRegister<TestRegister>();
		Reg.Register("素", async (o2) => NIL);
		var Case = Host.Children.Single(x => x.Data is not null).Data!;
		T(Case.TesteeTypes.Count == 0);
		T(Case.TesteeFnNames.Count == 0);
		return NIL;
	}
}