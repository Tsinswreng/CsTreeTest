using Tsinswreng.CsTreeTest;

namespace MyProj1.Test.MyDomains.Calculator;

public partial class TestCalculator {
	public void RegisterTryIntDivide(ITestNode Node) {
		// 最小寫法：連 testee metadata 都不傳（索引只在將來的過濾功能用到），
		// 骨架只剩「一次取 R + 註冊用例」。
		var R = Node.MkTestFnRegister<TestCalculator>().Register;
		var T = Assert.IsTrue;
		R("positive", async (o) => {
			T(Calculator.TryIntDivide(20, 4, out var R));
			T(R==5);
			return null;
		});
		R("Divide by zero", async (o) => {
			T(!Calculator.TryIntDivide(20, 0, out var R));
			return null;
		});
	}
}