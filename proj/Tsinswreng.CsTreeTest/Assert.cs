using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：只寫函數聲明（partial method）與 Doc；實現見 Assert.Impl.cs。
[Doc($$"""
#Sum[最簡斷言入口：條件不成立就拋異常。]
#Descr[
*失敗的表達方式就是拋異常*：
本庫不靠返回值表示成敗，
用例函數正常返回即算通過，拋出任何異常即算失敗（見 `{{nameof(TreeTestExecutor)}}`）。
所以斷言失敗時拋出的異常會被執行器收進
`{{nameof(TestCaseRunResult.Exception)}}`，不會中斷其餘用例——
詳細的失敗信息（異常的消息、堆棧）會被打印出來。

*為甚麼這麼薄*：
本類只提供最基礎的一條斷言。
各種領域斷言（相等、拋出指定異常、集合包含……）
應由使用方按自己的領域寫成工具方法，
不必都塞進本類。

用途分兩種：
+ 用例函數體裏直接調用，讓用例失敗。
+ 在 `{{nameof(ITester.RegisterTestsInto)}}` *之外*的地方（比如準備數據時）調用也可，
	但那時拋出的異常不歸執行器管，會照常往外冒。

*註冊用例時常寫成*
```cs
var T = {{nameof(Assert.IsTrue)}};
R("正數相加", async (o) => {
	T(_Calculator.Add(5, 3) == 8);
	return NIL;
});
```
]
""")]
public partial class Assert{
	[Doc($$"""
#Sum[斷言成立；不成立就拋帶 `ErrMsg` 的異常。]
#Descr[
*邊界條件*：
`Cond` 為真時*什麼都不做*、不產生開銷，正常返回 `NIL`；
為假時拋 `Exception`，消息就是 `ErrMsg`。

*`ErrMsg` 為空時*異常消息也是空字符串——
最終打印出來的失敗信息就只有異常類型與堆棧，
人看不出「哪個條件不成立」。
所以*給足 `ErrMsg`*：
寫清期望什麼、實際得到什麼，
例如 `$"期望 8，實際 {got}"`。

*已知取捨*：本方法拋的是最普通的 `Exception`，
不區分參數錯誤與斷言失敗；
需要區分時由使用方自己寫領域斷言。

目前這是唯一的斷言 API。
]
#Params([[斷言條件；為真即通過],
	[失敗時作為異常消息的文本；建議寫清期望值與實際值，
		為空時異常消息也是空字符串]])
#Rtn[`NIL`。成功時只表示「沒有拋異常」，調用方不需要接這個返回值。]
""")]
	public static partial nil IsTrue(bool Cond, str? ErrMsg = null);
}