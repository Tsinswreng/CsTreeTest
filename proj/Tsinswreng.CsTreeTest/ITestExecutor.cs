using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 單檔：ITestExecutor 接口；純聲明，無函數可分離。
[Doc($$"""
#Sum[測試執行器：跑一組節點，返回匯總。]
#Descr[
正常用法不必直接調 `Run`：
用 `{{nameof(ExtnTreeTestExecutor)}}` 的 `{{nameof(ExtnTreeTestExecutor.RunTests)}}` / `{{nameof(ExtnTreeTestExecutor.RunEtPrint)}}` 更方便
（可傳單個節點、會補默認選項、還能順帶打印）。
]
""")]
public interface ITestExecutor{
	[Doc($$"""
#Sum[跑一組節點。]
#Descr[
一次可給多個節點，
各節點掃描出來的用例*合併成同一組結果*、按 `{{nameof(TestCaseRunResult.Order)}}` 排序後返回——
所以既可以傳根跑整棵樹，
也可以只傳幾個子樹跑一部分。

*本方法不打印、也不因用例失敗而拋*：
失敗被收進 `{{nameof(TestCaseRunResult.Exception)}}`，
是否需要「有失敗就失敗」由調用方按 `{{nameof(TestRunSummary.Failed)}}` 判斷。

`Nodes` 為空列表時什麼都不跑，返回一個 `Total = 0` 的匯總（不拋）。
]
#Params([[要跑的根節點列表，不能為空引用；列表內元素的順序決定 `{{nameof(TestCaseRunResult.Order)}}` 的分配次序],
	[執行選項；傳空則用 `new OptTestExecutor()` 的默認值]])
#Rtn[本次執行的匯總，`{{nameof(TestRunSummary.Results)}}` 已按 `{{nameof(TestCaseRunResult.Order)}}` 排序]
""")]
	Task<TestRunSummary> Run(
		IList<ITestNode> Nodes
		,OptTestExecutor? Opt
	);
}
