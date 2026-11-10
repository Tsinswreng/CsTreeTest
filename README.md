## Tsinswreng.CsTreeTest

Tsinswreng.CsTreeTest 是一個用樹型結構組織測試用例的輕量測試框架。

它適合這類場景：

- 你希望按模塊、類、函數分層組織測試
- 你希望分文件寫測試，一個被測函數一個文件
- 你希望一次跑全部測試，也能只跑某個子樹或某一批用例
- 你希望在程式碼裏按測試類、被測類型、被測函數名篩選用例

### 安裝

```bash
dotnet add package Tsinswreng.CsTreeTest
```

### 依賴

此包依賴：

- `Tsinswreng.CsCore`
- `Tsinswreng.CsU128Id`

它**不依賴**任何依賴注入套件。測試類需要的依賴，
由各測試項目自己的全局依賴來源提供（見下文「全局依賴來源」）。

### 先看示例

想知道「測試類怎麼寫、怎麼註冊、怎麼組織」，直接看這兩處：

- 可跑的完整示例：`proj/Samples/MyProj1.Test/`
  入口 `Program.cs`（三步）、`MyProj1TestMgr.cs`（註冊測試類）、
  `MyDomains/Calculator/_TestCalculator.cs`（測試類主檔）、
  `TestAdd.cs` / `TestDivide.cs`（一個被測函數一個文件）。
- 框架源碼裏的 `[Doc]` 註釋也內嵌了同樣的完整示例，
  其中 `ITester` 的 `#Descr` 是「從寫測試類到跑起來」的全流程。

### 核心概念

#### ITestNode

`ITestNode` 是測試樹節點。一個節點可以：

- 帶一個測試用例 `Data`
- 帶多個子節點 `Children`
- 控制直接子節點是否按順序執行 `Ordered`（不遞歸）
- 控制整個遞歸子樹是否不參與並行 `NonParallelizable`（向下繼承）

#### ITestCase

`ITestCase` 表示單個測試用例。它除了保存真正要執行的 `FnTest` 之外，還保存：

- `TesterType`（屬於哪個測試類）
- `TesteeTypes`（測哪些被測類型）
- `TesteeFnNames`（測哪些被測函數名，用 `nameof` 賦值）
- `UniqName`（唯一名，同樹內不得重複）

元數據是查詢與篩選的依據，所以註冊時就要寫對。

#### ITester 與分文件寫法

`ITester` 表示「能向一個節點註冊測試」的類。通常是：

- 一個被測類對應一個測試類
- 測試類拆成多個 partial 文件：主文件 `_TestXxx.cs` 負責取依賴與串註冊方法，
  其餘文件每個只測一個被測函數

#### TestMgr

`TestMgr` 是測試管理器，取代舊版的 `DiEtTestMgr`。它負責：

- 持有根節點 `Root`
- 註冊測試類（`RegisterTester` / `RegisterTesterIntoBranch`）
- 建立 tester / testee / 函數名到節點的索引
- 收編其他測試管理器（`RegisterSubMgr`）
- 提供查詢入口

#### 全局依賴來源

框架不參與依賴的獲取。慣例是每個測試項目自備一個靜態 `Di` 類：

```csharp
public static class Di{
	public static IServiceProvider SvcProvider{
		get{ return field ?? throw new InvalidOperationException("Di.SvcProvider has not been initialized."); }
		set{ field = value ?? throw new ArgumentNullException(nameof(value)); }
	}

	public static T GetRSvc<T>() where T:class{
		return SvcProvider.GetRequiredService<T>();
	}
}
```

再配一個 `GlobalUsing.cs`：

```csharp
global using static MyProj1.Test.Di;
```

於是測試類在自己的欄位初始化時直接寫 `GetRSvc<ICalculator>()` 取被測對象，
不需要建構子注入。
`Di.SvcProvider` 必須在測試管理器的 `Init` 之前賦值，
因為 `Init` 會立刻實例化測試類。

#### TestFilter / TestSelection

篩選器。按測試類、被測類型、被測函數名、名稱片段、自訂謂詞挑用例，
挑出來的選集可以直接執行。篩選走索引，不重掃全樹。

#### TreeTestExecutor

執行器。負責跑一條子樹或一組節點，支持：

- 並行執行（默認開）
- 按樹層次切分 stage，跨層有屏障
- 在 `Ordered` 或 `NonParallelizable` 場景下順序執行
- 統計通過 / 失敗 / 耗時
- 輸出失敗詳情與總結

### 快速開始

```csharp
using Microsoft.Extensions.DependencyInjection;
using Tsinswreng.CsTreeTest;

public class Program{
	public static async Task<int> Main(string[] args){
		// 1) 組裝容器並交給全局依賴來源（必須在 Init 之前）。
		Di.SvcProvider = new ServiceCollection()
			.AddSingleton<ICalculator, Calculator>()
			.BuildServiceProvider();

		// 2) 初始化測試管理器：測試類此時被實例化、用例掛樹、索引建立。
		var mgr = MyProj1TestMgr.Inst;
		mgr.Init();

		// 3) 執行。
		var executor = new TreeTestExecutor();
		var Summary = await executor.RunEtPrint([mgr.Root]);
		return Summary.Failed > 0 ? 1 : 0;
	}
}
```

### 基本用法

1. 寫測試類主文件 `_TestCalculator.cs`

```csharp
using Tsinswreng.CsTreeTest;

namespace MyProj1.Test.MyDomains.Calculator;

public partial class TestCalculator : ITester{
	// 欄位初始化時取依賴，不需要建構子參數。
	private readonly ICalculator _Calculator = GetRSvc<ICalculator>();

	public ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterAdd(Node);
		RegisterTryIntDivide(Node);
		return Node;
	}
}
```

2. 按被測函數拆分測試文件 `TestAdd.cs`

```csharp
using Tsinswreng.CsTreeTest;

namespace MyProj1.Test.MyDomains.Calculator;

public partial class TestCalculator{
	public void RegisterAdd(ITestNode Node){
		// 元數據只在建註冊器時寫一遍，之後每條用例只寫名字與用例體。
		var R = Node.MkRegister<TestCalculator>(
			typeof(ICalculator),
			nameof(ICalculator.Add),
			"Add:"
		).Register;

		R("正數相加", async (o) => {
			Assert.IsTrue(_Calculator.Add(5, 3) == 8);
			return NIL;
		});
	}
}
```

不需要按函數篩選時，元數據可以不傳：

```csharp
var R = Node.MkRegister<TestCalculator>().Register;
```

3. 寫測試管理器並聲明測試類

```csharp
using Tsinswreng.CsTreeTest;

namespace MyProj1.Test;

public class MyProj1TestMgr : TestMgr{
	public static MyProj1TestMgr Inst = new();

	protected override void SetupTree(){
		RegisterTester<TestCalculator>();
		// 需要領域分組時：
		// RegisterTesterIntoBranch<TestCalculator>("Calculator");
	}
}
```

### 執行模型

- `Ordered = true` 時，當前節點的直接子節點按順序執行，不遞歸
- `NonParallelizable = true` 時，整個遞歸子樹都不參與並行，向下繼承
- 默認情況下，executor 並行跑同層各分支的可並行項

這很適合下面這類測試：

- 需要先插入測試數據，再做查詢與斷言，最後清理數據
- 某些節點可以安全並行，某些節點必須串行

### 篩選與查找

管理器把用例索引起來，按需挑選：

```csharp
// 按測試類找節點
var N = mgr.GetNodeByTesterType(typeof(TestCalculator));

// 按被測函數名找節點
var Nodes = mgr.GetNodesByTesteeFnName(nameof(ICalculator.Add));

// 篩選：測 ICalculator、名字帶「正數」的用例，先看命中再執行
var Hits = mgr.Filter()
	.TesteeType<ICalculator>()
	.NameContains("正數")
	.Select();
Hits.PrintHits();
await Hits.RunEtPrint();
```

### 適用場景

- 自研測試基礎設施
- 需要比單層測試列表更細緻的分層與分組
- 希望把測試器、依賴來源、用例索引整合在一起

### 參考

- 可跑示例：`proj/Samples/MyProj1.Test/`
- 框架自身測試（44 條用例）：`proj/Tsinswreng.CsTreeTest.Test/`
- 各公開 API 的用法都寫在源碼的 `[Doc(...)]` 註釋裏，
  配合 Typst 工具可直接生成文檔