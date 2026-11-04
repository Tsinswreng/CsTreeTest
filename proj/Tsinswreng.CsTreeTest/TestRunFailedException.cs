using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：TestRunFailedException 的成員宣告；實現見 TestRunFailedException.Impl.cs。
[Doc($$"""
「這次執行有失敗」的信號，異常消息即匯總。

只有 `{{nameof(TreeTestExecutorExtn.RunEtPrint)}}` 的 `ThrowOnFailed` 為真且有失敗時才拋；
逐條失敗原因看 `{{nameof(Summary)}}` 裏各條的 `{{nameof(TestCaseRunResult.Exception)}}`。
""")]
public sealed partial class TestRunFailedException : Exception{
	[Doc($$"""
產生本異常的那次執行的匯總；原樣記下，不複製。
""")]
	public TestRunSummary Summary{get;}

	[Doc($$"""
消息固定為「測試有失敗。總數＝…，通過＝…，失敗＝…」。
#Prm[{{nameof(Summary)}}][那次執行的匯總]
""")]
	public partial TestRunFailedException(TestRunSummary Summary);
}
