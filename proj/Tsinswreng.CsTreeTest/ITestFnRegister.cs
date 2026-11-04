using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 單檔：ITestFnRegister 接口；純聲明，無函數可分離。
[Doc($$"""
往一個節點上註冊用例的註冊器：元數據交一次，之後每條用例只寫名字與函數體。

用 `{{nameof(ITestFnRegisterExtn.MkRegister)}}` 造，並直接取 `{{nameof(Register)}}` 方法組；
存進局部變量再調用編譯不過（CS0149）。
```cs
var R = Node.{{nameof(ITestFnRegisterExtn.MkRegister)}}<TestCalculator>(typeof(ICalculator), nameof(ICalculator.Add), "Add:").{{nameof(Register)}};
R("正數相加", async (o) => { /* ... */ return NIL; });
```
""")]
public interface ITestFnRegister{
	[Doc($$"""
註冊一條用例；用例名為 `{{nameof(UniqNamePrefix)}} + {{nameof(UniqName)}}`。

元數據按調用時的欄位值快照，所以兩條用例之間改欄位會影響後面的用例。

#Prm[{{nameof(UniqName)}}][用例名，不含前綴；要能看出所測行爲，別用 `Test1`/`Try` 這種名字]
#Prm[{{nameof(Fn)}}][用例本體]
""")]
	void Register(str UniqName, FnTest Fn);

	[Doc($$"""
所屬測試類，由 `{{nameof(ITestFnRegisterExtn.MkRegister)}}` 的泛型參數寫入。
""")]
	Type TesterType{get;set;}

	[Doc($$"""
本節點下的用例測的被測類型。
""")]
	IList<Type> TesteeTypes{get;set;}

	[Doc($$"""
本節點下的用例測的被測函數名。
""")]
	IList<str> TesteeFnNames{get;set;}

	[Doc($$"""
自動加在用例名前的綴，用來避免全樹重名。
例：前綴用 `Add:`，失敗輸出就是 `Tests/Calculator/Add:正數相加`。
""")]
	str UniqNamePrefix{get;set;}
}
