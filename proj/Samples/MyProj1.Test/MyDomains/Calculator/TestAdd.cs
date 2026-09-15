using Tsinswreng.CsTreeTest;

namespace MyProj1.Test.MyDomains.Calculator;
// each part should only mainly test one function. in this part we test Add
public partial class TestCalculator {
	public void RegisterAdd(ITestNode Node) {
		// 新寫法：tester type 由泛型參數推出，testee types / fn names 一次性傳入，
		// 不用再寫 typeof(TestCalculator)，也不用先傳 [] 再改 register 屬性。
		var R = Node.MkTestFnRegister<TestCalculator>(
			[typeof(ICalculator)],
			[nameof(MyDomains.Calculator.ICalculator.Add)],
			"Add:"
		).Register;
		var T = Assert.IsTrue;
		R("Add positive numbers", async (o) => {
			T(Calculator.Add(5, 3)==8);
			T(Calculator.Add(6, 4)==10);
			return null;
		});
		R("Add positive and negative numbers", async (o) => {
			var r = Calculator.Add(5, -3);
			T(r==2);
			return null;
		});
	}
}