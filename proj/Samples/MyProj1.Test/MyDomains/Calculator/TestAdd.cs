using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace MyProj1.Test.MyDomains.Calculator;

[Doc("""
#Sum[測 `ICalculator.Add` 的分片。]

#Descr[
*本檔只測 `Add` 這一個被測函數。*
新增別的函數的測試時請另開文件
（例如測 `TryIntDivide` 的寫在 `TestDivide.cs`），
不要把不同函數的用例堆在同一檔。
]
""")]
public partial class TestCalculator{
	[Doc("""
#Sum[把 `Add` 的用例掛到節點上。]

#Descr[
註冊分兩段：
先 `MkRegister` 建註冊器（元數據只寫一遍），
再取它的 `Register` 方法組連註冊多條用例
（每條用例只寫名字與函數體）。

```cs
var R = Node.MkRegister<TestCalculator>(
	typeof(ICalculator),      // 被測類型
	nameof(ICalculator.Add),  // 被測函數名，必須用 nameof
	"Add:"                    // 用例名前綴
).Register;
R("正數相加", async (o) => { /* ... */ return NIL; });
```

*爲甚麼要寫 `.Register`*：
C# 不允許把註冊器對象存進局部變量後再 `R("名", fn)` 調用
（`CS0149：應輸入方法名稱`），
取到方法組才能這樣連着調。

*爲甚麼要寫前綴*：
同一棵樹內用例名不得重複；
本檔統一加 `Add:` 後，
失敗輸出的路徑就是
`Tests/TestCalculator/Add:正數相加`，
一眼能看出哪個被測函數的哪條用例失敗。
]
#Params([[要掛用例的節點，即測試類節點]])
""")]
	public void RegisterAdd(ITestNode Node){
		var R = Node.MkRegister<TestCalculator>(
			typeof(ICalculator)
			,nameof(ICalculator.Add)
			,"Add:"
		).Register;
		var T = Assert.IsTrue;

		R("正數相加", async (o) => {
			T(_Calculator.Add(5, 3) == 8);
			T(_Calculator.Add(6, 4) == 10);
			return NIL;
		});

		R("正負數相加", async (o) => {
			var Sum = _Calculator.Add(5, -3);
			T(Sum == 2);
			return NIL;
		});
	}
}