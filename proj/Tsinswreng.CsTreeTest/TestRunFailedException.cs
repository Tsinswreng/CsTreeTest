using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：TestRunFailedException 的成員宣告；實現見 TestRunFailedException.Impl.cs。
[Doc($$"""
#Sum[有失敗時拋出的例外，異常消息即匯總。]
#Descr[
*不是框架主動拋的*：
`Run` 與 `{{nameof(ExtnTreeTestExecutor.RunTests)}}` 從不因為用例失敗而拋；
只有 `{{nameof(ExtnTreeTestExecutor.RunEtPrint)}}` 的 `ThrowOnFailed` 為真、且 `Summary.Failed > 0` 時才拋本例外。

本例外不帶任何「哪條用例失敗」的信息，
逐條失敗原因在 `Summary.Results` 裏各條的 `Exception`——
本例外只是一個「這次執行有失敗」的信號。

是否要拋由調用方決定：
本庫默認只跑與報告，
進程的成敗由入口自己判斷。
]
""")]
public sealed partial class TestRunFailedException : Exception{
	[Doc($$"""
#Sum[產生本異常的那次執行的匯總。]
#Descr[
與異常消息同源（消息就是按它的三個計數拼的）；
需要逐條失敗詳情時用它，
不要去解析異常消息字符串。
]
""")]
	public TestRunSummary Summary{get;}

	[Doc($$"""
#Sum[用匯總造例外，消息為「總數／通過／失敗」。]
#Descr[
消息格式固定為
`測試有失敗。總數={Total}，通過={Passed}，失敗={Failed}`。

`Summary` 是*原樣*記下來的引用，不會複製，
所以拿到異常之後仍能訪問逐條結果。
]
#Params([[產生本異常的那次執行的匯總]])
""")]
	public partial TestRunFailedException(TestRunSummary Summary);
}
