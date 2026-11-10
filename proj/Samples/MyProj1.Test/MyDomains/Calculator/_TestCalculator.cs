using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace MyProj1.Test.MyDomains.Calculator;

[Doc("""
#Sum[測 `ICalculator` 的測試類主檔。]

#Descr[
*一個被測類對應一個測試類、一個目錄*，
測試類用 `partial class` 拆到多個文件：

```text
MyDomains/Calculator/
	Calculator.cs        被測類（真實工程裏在別的工程）
	_TestCalculator.cs   本檔：主檔
	TestAdd.cs           只測 ICalculator.Add
	TestDivide.cs        只測 ICalculator.TryIntDivide
```

*主檔只做兩件事*：
從全局依賴來源取被測對象、
把各分片的註冊方法依次串起來。
主檔自己不寫用例。

*每個分片只測一個被測函數*，
文件名即被測函數名，
這樣要測某個函數時知道去哪個文件找。
]
""")]
public partial class TestCalculator : ITester{
	[Doc("""
#Sum[被測對象，欄位初始化時就向全局依賴來源取。]

#Descr[
因此*全局依賴來源必須在測試管理器 `Init()` 之前就緒*：
`Init()` 會立刻 `new` 本測試類，
本欄位的初始化就會向 `Di.GetRSvc` 取對象，
而那時 `Di.SvcProvider` 若還沒賦值就會抛異常。

框架不參與取依賴：
它只負責把測試類 `new` 出來。
]
""")]
	private readonly ICalculator _Calculator = GetRSvc<ICalculator>();

	[Doc("""
#Sum[把本測試類的全部用例掛到給定節點上。]

#Descr[
本方法由測試管理器在 `Init()` 時調用，
`Node` 一定非空（就是掛本測試類的那個節點）。
`Node ??= new TestNode()` 是爲了讓本測試類也能脫離測試樹單獨跑。

執行方式可在本節點上調整：
+ 想讓本節點的直接子節點依序跑（例如先插數據、再測、最後清理）：
	`Node.Ordered = true;`
+ 想讓整棵子樹都不與別的分支同時跑：
	`Node.NonParallelizable = true;`

純函數用例互不干擾，
所以本示例不設這兩個開關，
讓它們按默認的並行方式跑。
]
#Params([[框架給的掛載點；測試管理器一定傳非空值]])
#Rtn[掛好用例的那個節點]
""")]
	public ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();

		// 依次調用各分片的註冊方法。
		// 新增一個被測函數的測試時，
		// 就在本檔多加一行。
		RegisterAdd(Node);
		RegisterTryIntDivide(Node);
		return Node;
	}
}