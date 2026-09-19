using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace MyProj1.Test.MyDomains.Calculator;

[Doc("""
#Sum[測 `ICalculator.TryIntDivide` 的分片。]

#Descr[
*本檔只測 `TryIntDivide` 這一個被測函數。*

本檔順便示範*最簡寫法*：
若不需要按被測函數篩選這些用例，
`MkRegister` 的元數據可以完全不傳，
註冊骨架就只剩「取 `R` + 註冊用例」兩行：

```cs
var R = Node.MkRegister<TestCalculator>().Register;
R("除數爲零返回 false", async (o) => { /* ... */ return NIL; });
```
]
""")]
public partial class TestCalculator{
	[Doc("""
#Sum[把 `TryIntDivide` 的用例掛到節點上。]

#Descr[
不傳元數據時，
用例的 `TesteeTypes`／`TesteeFnNames` 就是空列表，
按被測類型或函數名篩選時不會命中它們。

注意：不傳元數據時*一個參數都不要傳*。
寫成 `MkRegister<TestCalculator>(null, null)` 會因重載歧義編譯不過
（`CS0121`）。
]
#Params([[要掛用例的節點，即測試類節點]])
""")]
	public void RegisterTryIntDivide(ITestNode Node){
		var R = Node.MkRegister<TestCalculator>().Register;
		var T = Assert.IsTrue;

		R("整數相除", async (o) => {
			T(_Calculator.TryIntDivide(20, 4, out var Q1));
			T(Q1 == 5);
			return NIL;
		});

		R("除數為零返回 false", async (o) => {
			T(!_Calculator.TryIntDivide(20, 0, out var Q2));
			return NIL;
		});
	}
}