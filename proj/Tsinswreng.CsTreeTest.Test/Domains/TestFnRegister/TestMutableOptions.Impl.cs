using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.TestFnRegister;

public partial class TestTestFnRegister{
	public partial void RegisterMutableOptions(ITestNode Node){
		var T = Assert.IsTrue;

		var Reg = Node.MkTestFnRegister<TestTestFnRegister>();
		Reg.Register("first", async (o)=>{ return null; });
		Reg.UniqNamePrefix = "P2:";
		Reg.Register("second", async (o)=>{
			var First = Node.Children[0].Data!;
			var Second = Node.Children[1].Data!;
			T(First.UniqName == "first");
			T(Second.UniqName == "P2:second");
			T(Second.TesterType == typeof(TestTestFnRegister));
			return null;
		});
	}
}