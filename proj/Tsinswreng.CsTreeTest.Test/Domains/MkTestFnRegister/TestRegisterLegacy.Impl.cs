using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.MkTestFnRegister;

public partial class TestMkTestFnRegister{
	public partial void RegisterLegacy(ITestNode Node){
		var R = Node.MkTestFnRegister(
			typeof(TestMkTestFnRegister),
			[typeof(TestNode)],
			[nameof(TestNode.UniqName)],
			"L:"
		).Register;
		var T = Assert.IsTrue;

		R("舊 4 參數重載仍可用", async (o)=>{
			var Case = Node.Children[2].Data!;
			T(Case.TesterType == typeof(TestMkTestFnRegister));
			T(Case.TesteeTypes.SequenceEqual([typeof(TestNode)]));
			T(Case.TesteeFnNames.SequenceEqual([nameof(TestNode.UniqName)]));
			T(Case.UniqName == "L:" + "舊 4 參數重載仍可用");
			return null;
		});
	}
}