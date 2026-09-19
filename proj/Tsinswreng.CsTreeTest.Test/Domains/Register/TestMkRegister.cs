using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Register;

/// 測 MkRegister 的元數據如何流進用例。
public partial class TestRegister{
	/// 把元數據用例掛到節點上。
	public void RegisterMkRegister(ITestNode Node){
		// 本領域測的就是「元數據會帶進用例」，
		// 因此本檔自己就用這個機制註冊用例。
		var R = Node.MkRegister<TestRegister>(
			typeof(IFakeSvc)
			,nameof(IFakeSvc.Echo)
			,"Meta:"
		).Register;
		var T = Assert.IsTrue;

		R("從全局依賴來源取到的服務可用", async (o) => {
			T(_Svc.Echo(11) == 11);
			return NIL;
		});

		R("元數據按註冊器設定帶進每條用例", async (o) => {
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
		});

		R("不傳元數據時列表為空", async (o) => {
			var Host = new TestNode{UniqName = "EmptyHost"};
			var Reg = Host.MkRegister<TestRegister>();
			Reg.Register("素", async (o2) => NIL);
			var Case = Host.Children.Single(x => x.Data is not null).Data!;
			T(Case.TesteeTypes.Count == 0);
			T(Case.TesteeFnNames.Count == 0);
			return NIL;
		});
	}
}