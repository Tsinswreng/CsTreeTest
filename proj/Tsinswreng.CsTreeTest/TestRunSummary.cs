using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：TestRunSummary 的成員宣告；實現見 TestRunSummary.Impl.cs。
[Doc($$"""
#Sum[一次執行的匯總統計。]
#Descr[
不管有沒有失敗，`{{nameof(ITestExecutor.Run)}}` 都返回本對象；
失敗的詳情在 `{{nameof(TestRunSummary.Results)}}` 裏逐條看（或直接用 `{{nameof(TestRunSummary.FailedResults)}}()`）。
]
""")]
public sealed partial class TestRunSummary{
	[Doc(@$"#Sum[本次執行的開始時間。在收集階段之前記下，所以收集耗時也計在總耗時裏。]")]
	public DateTimeOffset StartedAt{get;set;}

	[Doc(@$"#Sum[本次執行的結束時間。所有用例跑完並匯總完之後記下。]")]
	public DateTimeOffset EndedAt{get;set;}

	[Doc(@$"#Sum[總耗時，即 `{nameof(EndedAt)} - {nameof(StartedAt)}`（按需算，不是存下來的字段）。]")]
	public TimeSpan Elapsed => EndedAt - StartedAt;

	[Doc(@$"#Sum[本次實際跑到的用例數，即 `{nameof(Results)}.Count`。沒有被掃到的節點不計。]")]
	public int Total{get;set;}

	[Doc(@$"#Sum[通過數。]")]
	public int Passed{get;set;}

	[Doc(@$"#Sum[失敗數，即 `{nameof(Total)} - {nameof(Passed)}`。入口通常就靠它決定進程成敗。]")]
	public int Failed{get;set;}

	[Doc($$"""
#Sum[逐條用例的結果，*按 `{{nameof(TestCaseRunResult.Order)}}`（樹的先序）排列*。]
#Descr[
不是按完成順序排列——
`{{nameof(TreeTestExecutor)}}` 的執行流程返回前已按 `{{nameof(TestCaseRunResult.Order)}}` 排過序，
所以並行與否都不影響這裏的順序。
]
""")]
	public IList<TestCaseRunResult> Results{get;set;} = [];

	[Doc($$"""
#Sum[只取失敗那幾條（保持 `{{nameof(TestRunSummary.Results)}}` 的順序）。]
#Descr[
供調用方自行出報告時用，
不必自己去 `{{nameof(TestRunSummary.Results)}}` 裏篩。

注意本方法會把結果複製成新列表，
所以不要在熱路徑上反覆調用。
]
#Rtn[失敗用例的結果列表；全部通過時為空列表]
""")]
	public partial IList<TestCaseRunResult> FailedResults();
}
