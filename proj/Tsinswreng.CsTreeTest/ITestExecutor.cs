using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 單檔：ITestExecutor 接口；純聲明，無函數可分離。
[Doc($$"""
測試執行器：跑一組節點，返回匯總。
正常用法用 `{{nameof(TreeTestExecutorExtn.RunTests)}}` / `{{nameof(TreeTestExecutorExtn.RunEtPrint)}}`，本接口只需在自寫執行器時實現。
""")]
public interface ITestExecutor{
	[Doc($$"""
跑一組節點，各節點的用例併成同一份匯總。

不打印、也不因用例失敗而拋：失敗收在 `{{nameof(TestCaseRunResult.Exception)}}`。
`{{nameof(Nodes)}}` 為空列表時不跑任何東西，返回 `Total = 0` 的匯總。

#Prm[{{nameof(Nodes)}}][要跑的根節點，順序決定 `{{nameof(TestCaseRunResult.Order)}}` 的分配]
#Prm[{{nameof(Opt)}}][為空時用默認選項]
#Rtn[匯總；`{{nameof(TestRunSummary.Results)}}` 已按先序排好]
""")]
	Task<TestRunSummary> Run(
		IList<ITestNode> Nodes
		,OptTestExecutor? Opt
	);
}
