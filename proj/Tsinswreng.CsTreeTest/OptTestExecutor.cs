using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 單檔：OptTestExecutor 純數據類型；無函數體可分離。
[Doc($$"""
執行選項。三個都是公開字段，只改關心的那幾個。

`{{nameof(ITestExecutor.Run)}}` 的 `Opt` 傳空時用本類型的默認值。
```cs
new TreeTestExecutor().RunEtPrint(root, Opt: new OptTestExecutor{IsParallel = false, Arg = myFixture});
```
""")]
public class OptTestExecutor{
	[Doc($$"""
是否並行；默認 `true`。為假時整棵樹單線程按收集順序（先序）跑。

這是全局總閘，與 `{{nameof(ITestNode.IsParallelRecursive)}}` 互相獨立；任一要求不並行，該項就不並行。
""")]
	public bool IsParallel = true;

	[Doc($$"""
最大並行度；默認 `Environment.ProcessorCount`。

`<= 0` 也退回到處理器數；要不並行請用 `{{nameof(IsParallel)}} = false`。
""")]
	public int MaxDegreeOfParallelism = Environment.ProcessorCount;

	[Doc($$"""
傳給每條用例的參數；所有用例共用同一個引用，不需要就忽略。

擴展方法的 `Arg` 參數只在本字段為空時才寫進來（`Opt.Arg ??= Arg`），即本字段優先。
""")]
	public obj? Arg = default;
}
