using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

[Doc("""
#Sum[測試類（tester）要實現的接口：
把自己要跑的用例掛到給定的樹節點上。]

#Descr[
註冊與組織的全流程如下。
倉庫裏可跑的完整示例在 `proj/Samples/MyProj1.Test/`，
框架自身測試在 `proj/Tsinswreng.CsTreeTest.Test/`，
兩者的組織方式相同。

*第一步：一個被測類一個目錄，測試類拆成多個文件。*
測試類用 `partial class`，
以 `_TestXxx.cs` 為主檔——
主檔不寫用例，
只做兩件事：
從全局依賴來源取被測對象、
依次調用各分片的註冊方法。
其餘每個文件只測*一個被測函數*，
文件名即被測函數名。

```cs
MyDomains/Calculator/
	Calculator.cs        被測類（真實工程裏在別的工程）
	_TestCalculator.cs   主檔：取依賴 + 組裝各分片
	TestAdd.cs           只測 ICalculator.Add
	TestDivide.cs        只測 ICalculator.TryIntDivide
```

*第二步：主檔長這樣。*
`GetRSvc<T>()` 是測試項目自己的全局依賴來源，
框架不參與依賴的獲取，
只負責把本類 `new` 出來。

```cs
// _TestCalculator.cs
using Tsinswreng.CsTreeTest;

namespace MyProj1.Test.MyDomains.Calculator;

public partial class TestCalculator : ITester{
	// 欄位初始化時取被測對象，不需要建構子參數。
	private readonly ICalculator _Calculator = GetRSvc<ICalculator>();

	public ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterAdd(Node);
		RegisterTryIntDivide(Node);
		return Node;
	}
}
```

*第三步：每個被測函數一個文件。*
`MkRegister` 一次寫好元數據（測試類、被測類型、被測函數名、用例名前綴），
之後每條用例只寫名字與函數體。
用例名在同一個測試類內要能看出所測行爲。

```cs
// TestAdd.cs
using Tsinswreng.CsTreeTest;

namespace MyProj1.Test.MyDomains.Calculator;

public partial class TestCalculator{
	public void RegisterAdd(ITestNode Node){
		var R = Node.MkRegister<TestCalculator>(
			typeof(ICalculator)        // 被測類型
			,nameof(ICalculator.Add)   // 被測函數名，必須用 nameof
			,"Add:"                    // 用例名前綴
		).Register;
		var T = Assert.IsTrue;

		R("正數相加", async (o) => {
			T(_Calculator.Add(5, 3) == 8);
			return NIL;
		});
	}
}
```

*第四步：在測試管理器裏聲明本測試類。*
測試類自己不進測試樹；
由本項目的 `TestMgr` 在 `SetupTree` 裏聲明，
`Init()` 時才統一 `new` 出來並回調 `RegisterTestsInto`。

```cs
// MyProj1TestMgr.cs
public class MyProj1TestMgr : TestMgr{
	public static MyProj1TestMgr Inst = new();

	protected override void SetupTree(){
		// 直接掛在根下：
		RegisterTester<TestCalculator>();
		// 或歸到領域分組下（分組節點自動建）：
		// RegisterTesterIntoBranch<TestCalculator>("Calculator");
	}
}
```

*第五步：入口三步。*
順序不能顛倒：
依賴來源必須先就緒，
因爲 `Init()` 會立刻實例化測試類，
而測試類在欄位初始化時就取依賴。

```cs
Di.SvcProvider = new ServiceCollection()
	.AddSingleton<ICalculator, Calculator>()
	.BuildServiceProvider();

MyProj1TestMgr.Inst.Init();

await new TreeTestExecutor().RunEtPrint([MyProj1TestMgr.Inst.Root]);
```

*多個測試工程怎麼合併。*
上層只需要收編下層的管理器，
不必逐個註冊下層的測試類：

```cs
public class AllTestMgr : TestMgr{
	protected override void SetupTree(){
		RegisterSubMgr(MyProj1TestMgr.Inst);
		RegisterSubMgr(MyProj2TestMgr.Inst);
	}
}
```
]
""")]
public interface ITester{
	[Doc("""
#Sum[把自己要註冊的用例掛到 `Node` 上，再把 `Node` 返回。]
#Descr[
`Node` 為空時自行新建一個節點並返回，
這樣調用方可以脫離測試樹單獨跑本測試類。
實現本方法時只負責掛用例，
不要在這裏跑用例。
]
#Params([[框架給的掛載點；測試管理器一定傳非空值，
	只在單獨調用本方法時才可能為空]])
#Rtn[掛好用例的那個節點，通常就是傳進來的 `Node`]
""")]
	ITestNode RegisterTestsInto(ITestNode? Node);
}