using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 單檔：ITestFnRegister 接口；純聲明，無函數可分離。
[Doc($$"""
#Sum[往測試樹的一個節點上註冊用例的註冊器。]
#Descr[
同一個測試類的各條用例共享同一批元數據
（屬於哪個測試類、測哪些被測類型與被測函數），
所以元數據只在建註冊器時交一次，
之後每條用例只寫用例名與用例體。

正常用法不必直接碰本接口：
用擴展方法 `{{nameof(ExtnITestFnRegister.MkRegister)}}` 造出來，
直接取它的 `{{nameof(ITestFnRegister.Register)}}` 方法組來連註冊多條用例：

```cs
var R = Node.{{nameof(ExtnITestFnRegister.MkRegister)}}<TestCalculator>(
	typeof(ICalculator),
	nameof(ICalculator.Add),
	"Add:"
).{{nameof(ITestFnRegister.Register)}};

R("正數相加", async (o) => { /* ... */ return NIL; });
R("負數相加", async (o) => { /* ... */ return NIL; });
```

*為甚麼要取方法組*
（`.{{nameof(ITestFnRegister.Register)}}` 而不是把註冊器對象存進局部變量）：
C# 只允許直接調用委託或成員訪問，
把註冊器存進局部變量後再 `R("名", fn)` 會編譯不過
（`CS0149：應輸入方法名稱`）。
]
""")]
public interface ITestFnRegister{
	[Doc($$"""
#Sum[註冊一條用例。]
#Descr[
用例的元數據取本註冊器*當前*的欄位值，
用例名為 `{{nameof(UniqNamePrefix)}} + {{nameof(UniqName)}}`。

由於元數據是註冊時快照的，
可以在兩條用例之間改本註冊器的欄位
（例如換一個前綴），
後面的用例就會用新值。

調用方式見接口註釋：
先 `var R = Node.{{nameof(ExtnITestFnRegister.MkRegister)}}<...>(...).{{nameof(ITestFnRegister.Register)}};`
再 `R("用例名", 用例體)`。
]
#Params([[用例名，不含前綴；應能看出所測行爲，禁止 `Test1`、`Try`、`Temp` 這類名字],
	[用例函數本體，簽名為 `Task<obj?> (obj? Arg)`]])
""")]
	void Register(str UniqName, FnTest Fn);

	[Doc($$"""
#Sum[本節點下的用例屬於哪個測試類。]
#Descr[
由 `{{nameof(ExtnITestFnRegister.MkRegister)}}<TTester>` 的泛型參數寫入。
]
""")]
	Type TesterType{get;set;}

	[Doc($$"""
#Sum[本節點下的用例測的是哪些被測類型。]
""")]
	IList<Type> TesteeTypes{get;set;}

	[Doc($$"""
#Sum[本節點下的用例測的是哪些被測函數名。]
""")]
	IList<str> TesteeFnNames{get;set;}

	[Doc($$"""
#Sum[自動加在用例名前面的前綴。]
#Descr[
同一個被測類的用例散在多個節點時，
用前綴區分，
使全樹的用例名不重複。

例：測 `Add` 的文件用 `"Add:"`，
失敗輸出就會是
`Tests/Calculator/TestCalculator/Add:正數相加`。
]
""")]
	str UniqNamePrefix{get;set;}
}
