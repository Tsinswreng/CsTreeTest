using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 單檔：OptTestExecutor 純數據類型；無函數體可分離。
[Doc($$"""
#Sum[執行選項。]
#Descr[
三個字段都是公開*字段*（不是屬性），
直接寫字段初始化器即可，
只改關心的那幾個：

```cs
await new TreeTestExecutor().RunEtPrint(root, Opt: new OptTestExecutor{
	IsParallel = false,
	MaxDegreeOfParallelism = 4,
	Arg = myFixture,
});
```

`{{nameof(ITestExecutor.Run)}}` 的 `Opt` 傳空時會自動用 `new OptTestExecutor()`，
所以本類型的所有默認值就是框架的默認行為。
]
""")]
public class OptTestExecutor{
	[Doc($$"""
#Sum[是否並行；默認 `true`。]
#Descr[
為真時同一階段內*可並行*的項走 `Parallel.ForEachAsync` 同時跑；
為假時整棵樹退化成單線程按收集順序（先序）跑，
`{{nameof(OptTestExecutor.MaxDegreeOfParallelism)}}` 也一併失效。

*這是全局總閘*：
`{{nameof(ITestNode.IsParallelRecursive)}}` 是逐子樹的閘，兩者互相獨立、可同時使用；
一項只要*任一*閘要求不並行，它就不並行。
]
""")]
	public bool IsParallel = true;

	[Doc($$"""
#Sum[最大同時執行數；默認 `Environment.ProcessorCount`。]
#Descr[
原樣傳給 `ParallelOptions.MaxDegreeOfParallelism`。

`<= 0` 時同樣退回到 `Environment.ProcessorCount`（見 `{{nameof(TreeTestExecutor.Run)}}`），
所以*不要用 0 或負數表示「不並行」*——
要整個執行不並行請用 `IsParallel = false`。

只有 `{{nameof(OptTestExecutor.IsParallel)}}` 為真時本字段才有意義。
]
""")]
	public int MaxDegreeOfParallelism = Environment.ProcessorCount;

	[Doc($$"""
#Sum[傳給用例函數的參數；默認空。]
#Descr[
會*原樣*傳給每條用例的函數簽名最後一個參數（即 `{{nameof(FnTest)}}` 的 `Arg`），
所有用例拿到的是同一個引用，
用例不需要時忽略即可。

注意優先級：
擴展方法 `{{nameof(TreeTestExecutorExtn.RunTests)}}` / `{{nameof(TreeTestExecutorExtn.RunEtPrint)}}` 的 `Arg` 參數只在*本字段為空*時才寫進來
（`Opt.Arg ??= Arg`），
即本字段優先。
]
""")]
	public obj? Arg = default;
}
