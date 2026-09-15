using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.MkTestFnRegister;

public partial class TestMkTestFnRegister{
	public partial void RegisterDefaults(ITestNode Node){
		var R = Node.MkTestFnRegister<TestMkTestFnRegister>().Register;
		var T = Assert.IsTrue;

		R("不傳 metadata 時默認空列表", async (o)=>{
			var Case = Node.Children[1].Data!;
			T(Case.TesterType == typeof(TestMkTestFnRegister));
			T(Case.TesteeTypes.Count == 0);
			T(Case.TesteeFnNames.Count == 0);
			T(Case.UniqName == "不傳 metadata 時默認空列表");
			return null;
		});
	}
}