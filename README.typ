#import "@preview/tsinswreng-auto-heading:0.1.0": auto-heading
#let H = auto-heading;

#H[Tsinswreng.CsTreeTest][
	Tsinswreng.CsTreeTest 是一個用樹型結構組織測試用例的輕量測試框架。

	它適合這類場景：

	- 你希望按模塊、類、函數分層組織測試
	- 你希望一次跑全部測試，也能只跑某個子樹
	- 你希望對測試器、被測類型、被測函數建立索引
	- 你需要同時支持同步和異步測試函數

	#H[安裝][
		```bash
		dotnet add package Tsinswreng.CsTreeTest --version 0.0.1-alpha
		```
	]

	#H[依賴][
		此包依賴：

		- `Tsinswreng.CsCore`
		- `Tsinswreng.CsU128Id`
		- `Microsoft.Extensions.DependencyInjection.Abstractions`
	]

	#H[核心概念][
		#H[ITestNode][
			`ITestNode` 是測試樹節點。

			一個節點可以：

			- 帶一個測試用例 `Data`
			- 帶多個子節點 `Children`
			- 控制是否按順序執行 `Ordered`
			- 控制整個遞歸子樹是否禁並行 `NonParallelizable`
		]

		#H[ITestCase][
			`ITestCase` 表示單個測試用例。

			它除了保存真正要執行的 `FnTest` 之外，還保存：

			- `TesterType`
			- `TesteeTypes`
			- `TesteeFnNames`
			- `UniqName`
		]

		#H[ITester][
			`ITester` 表示“能向一個節點中註冊測試”的類。

			你通常爲一個被測類寫一個 tester，
			然後把不同函數的測試拆到不同 partial 文件中。
		]

		#H[DiEtTestMgr][
			`DiEtTestMgr` 是帶依賴注入能力的測試管理器。

			它負責：

			- 持有根節點 `TestNode`
			- 註冊 tester 類型
			- 建立 tester / testee / 函數名到節點的索引
			- 與 `IServiceCollection`、`IServiceProvider` 協作
			- 收編其他子測試管理器
		]

		#H[TreeTestExecutor][
			`TreeTestExecutor` 負責執行測試樹。

			它支持：

			- 並行執行
			- 按樹層次切分 stage
			- 在 `Ordered` 或 `NonParallelizable` 場景下順序執行
			- 統計通過/失敗/耗時
			- 輸出失敗詳情與總結
		]
	]

	#H[快速開始][
		```csharp
		using Microsoft.Extensions.DependencyInjection;
		using Tsinswreng.CsTreeTest;

		public class Program{
			public static IServiceCollection SvcColct = new ServiceCollection();
			public static IServiceProvider SvcProvdr = null!;

			public static async Task Main(string[] args){
				var mgr = MyProj1TestMgr.Inst;
				SvcProvdr = mgr.InitSvc(SvcColct, sc => sc.BuildServiceProvider());
				ITestExecutor executor = new TreeTestExecutor();
				await executor.RunEtPrint(mgr.TestNode);
			}
		}
		```
	]

	#H[基本用法][
		#H[1. 寫 tester 主文件][
			```csharp
			using Tsinswreng.CsTreeTest;

			namespace MyProj1.Test.MyDomains.Calculator;

			public partial class TestCalculator : ITester {
				ICalculator Calculator;

				public TestCalculator(ICalculator Calculator) {
					this.Calculator = Calculator;
				}

				public ITestNode RegisterTestsInto(ITestNode? Node) {
					Node ??= new TestNode();
					Node.Ordered = false;
					Node.NonParallelizable = false;
					RegisterAdd(Node);
					RegisterTryIntDivide(Node);
					return Node;
				}
			}
			```
		]

		#H[2. 按函數拆分測試文件][
			```csharp
			using Tsinswreng.CsTreeTest;

			namespace MyProj1.Test.MyDomains.Calculator;

			public partial class TestCalculator {
				public void RegisterAdd(ITestNode Node) {
					var register = Node.MkTestFnRegister(
						typeof(TestCalculator),
						[typeof(ICalculator)],
						[nameof(ICalculator.Add)],
						"Add:"
					);
					var R = register.Register;

					R("positive", async _ => {
						var r = Calculator.Add(5, 3);
						if(r != 8){
							throw new Exception($"Expected 8 but got {r}");
						}
						return null;
					});
				}
			}
			```
		]

		#H[3. 寫 TestMgr][
			```csharp
			using Tsinswreng.CsTreeTest;

			namespace MyProj1.Test;

			public class MyProj1TestMgr : DiEtTestMgr {
				public static MyProj1TestMgr Inst = new();

				public override ITestNode RegisterTestsInto(ITestNode? Node) {
					Node = this.TestNode;
					this.RegisterTester<TestCalculator>();
					return Node;
				}
			}
			```
		]
	]

	#H[執行模型][
		- `Ordered = true` 時，當前節點的直接子節點按順序執行
		- `NonParallelizable = true` 時，整個遞歸子樹都不並行
		- 默認情況下，executor 會盡量並行跑不同 stage 的可並行項

		這很適合下面這類測試：

		- 需要先插入測試數據，再做查詢與斷言，最後清理數據
		- 某些節點可以安全並行，某些節點必須串行
	]

	#H[索引與查找][
		`DiEtTestMgr` 會維護多組索引：

		- `UniqName_TestNode`
		- `TesterType_TestNode`
		- `TesteeType_TestNode`
		- `TesteeFnName_TestNode`

		因此你可以：

		- 按 tester type 找到測試子樹
		- 按被測類型找相關用例
		- 按被測函數名找相關用例
	]

	#H[適用場景][
		- 自研測試基礎設施
		- 需要比單層測試列表更細緻的分層與分組
		- 希望把測試器、依賴注入、用例索引整合在一起
	]

	#H[參考][
		- `Doc/Spec/Test.typ`
		- `Doc/Spec/TestSample.typ`
		- `proj/Samples/MyProj1.Test/`
	]
]

// 不使用註解做測試發現、需要手動註冊測試用例

// 測試用例以樹型結構組織

// 每個節點可以只有一個測試用例(只跑一個函數)
// 也可以有多個測試用例

// 樹節點劃分的粒度:

// ```
// 根/
// 	領域節點(可選)/
// 		文件夾(可選)/
// 			類/
// 				自定義聚合(可選)/
// 					單個函數
// ```

// 樹型的好處:

// 用戶可選擇一次性運行所有測試用例(即運行根節點)，也可以選擇指定運行一部分(相關的)測試用例
// 用樹型結構組織更方便

