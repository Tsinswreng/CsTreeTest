using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.MkTestFnRegister;

public partial class TestMkTestFnRegister{
	public partial void RegisterGenericMeta(ITestNode Node){
		var R = Node.MkTestFnRegister<TestMkTestFnRegister>(
			[typeof(TestNode)],
			[nameof(TestNode.UniqName)],
			"G:"
		).Register;
		var T = Assert.IsTrue;

		R("泛型重載推斷 tester type 且 metadata 一次性生效", async (o)=>{
			var Case = Node.Children[0].Data!;
			T(Case.TesterType == typeof(TestMkTestFnRegister));
			T(Case.TesteeTypes.SequenceEqual([typeof(TestNode)]));
			T(Case.TesteeFnNames.SequenceEqual([nameof(TestNode.UniqName)]));
			T(Case.UniqName == "G:" + "泛型重載推斷 tester type 且 metadata 一次性生效");
			return null;
		});
	}
}