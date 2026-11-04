using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：TestRunSummary 的成員宣告；實現見 TestRunSummary.Impl.cs。
[Doc($$"""
一次執行的匯總；不管有沒有失敗都返回本對象，詳情看 `{{nameof(Results)}}` 或 `{{nameof(FailedResults)}}`。
""")]
public sealed partial class TestRunSummary{
	[Doc($$"""
開始時間；在收集階段之前記下，所以收集耗時也計在總耗時裏。
""")]
	public DateTimeOffset StartedAt{get;set;}

	[Doc($$"""
結束時間。
""")]
	public DateTimeOffset EndedAt{get;set;}

	[Doc($$"""
總耗時，即 `{{nameof(EndedAt)}} - {{nameof(StartedAt)}}`。
""")]
	public TimeSpan Elapsed => EndedAt - StartedAt;

	[Doc($$"""
實際跑到的用例數。
""")]
	public int Total{get;set;}

	[Doc($$"""
通過數。
""")]
	public int Passed{get;set;}

	[Doc($$"""
失敗數；入口通常靠它決定進程成敗。
""")]
	public int Failed{get;set;}

	[Doc($$"""
逐條結果，按 `{{nameof(TestCaseRunResult.Order)}}`（樹的先序）排列，不是完成順序。
""")]
	public IList<TestCaseRunResult> Results{get;set;} = [];

	[Doc($$"""
只取失敗那幾條，保持原次序；會複製成新列表，別在熱路徑反覆調用。
#Rtn[失敗結果列表，全過時為空列表]
""")]
	public partial IList<TestCaseRunResult> FailedResults();
}
