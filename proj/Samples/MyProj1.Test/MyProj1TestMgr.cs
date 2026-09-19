using MyProj1.Test.MyDomains.Calculator;
using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace MyProj1.Test;

[Doc("""
#Sum[本測試工程的測試管理器：聲明本工程有哪些測試類。]

#Descr[
*每個測試工程的 csproj 都要有一個 TestMgr*，
本工程的全部測試類都在 `SetupTree` 裏聲明。

爲甚麼要分兩步（先聲明、後初始化）：
`SetupTree` 只是*聲明*要註冊哪些測試類，
真正 `new` 出測試類、讓它們把用例掛到樹上、
以及建立索引，
都發生在入口調用 `Init()` 的時候。
這樣測試類的實例化時機就一定晚於依賴來源的就緒時機。

多個測試工程要合併時，
上層 TestMgr 不必逐個註冊下層的測試類，
只收編下層的管理器即可：

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
public class MyProj1TestMgr : TestMgr{
	[Doc("""
#Sum[本管理器的全局唯一實例。]

#Descr[
寫成靜態欄位是慣例：
各處（入口、上層管理器）都用同一個管理器。

這裏不會有「靜態初始化時就開始建樹」的時序問題，
因爲樹是在 `Init()` 裏才建的，
測試類也是那時才實例化的。
]
""")]
	public static MyProj1TestMgr Inst = new();

	[Doc("""
#Sum[聲明本工程要註冊哪些測試類。]

#Descr[
只*聲明*，不在這裏 `new` 測試類，
也不在這裏跑用例。

兩種聲明方式：
+ `RegisterTester<T>()`：直接掛在根下。
+ `RegisterTesterIntoBranch<T>("分組名")`：掛到領域分組之下，
	分組節點會自動建在根下，
	需要單跑某個領域時以該分組爲篩選範圍即可。

同名測試類只能聲明一次；
重複聲明會在掛樹建索引時撞上唯一名檢查並抛異常。
]
""")]
	protected override void SetupTree(){
		this.RegisterTester<TestCalculator>();
	}
}