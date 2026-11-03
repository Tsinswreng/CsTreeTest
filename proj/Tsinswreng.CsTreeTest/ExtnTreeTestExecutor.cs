using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：ExtnTreeTestExecutor 擴展方法的宣告；實現見 ExtnTreeTestExecutor.Impl.cs。
[Doc($$"""
#Sum[執行器的便捷入口。]
#Descr[
四個重載按兩件事組合：
*跑單個節點還是節點列表*（`{{nameof(ExtnTreeTestExecutor.RunTests)}}` 與 `{{nameof(ExtnTreeTestExecutor.RunEtPrint)}}` 各有兩種），
以及*要不要跑完打印*。

共同約定：
+ `{{nameof(ITestMgr.Root)}}` / `Nodes` 不能為空引用，為空直接拋 `ArgumentNullException`。
+ `Opt` 傳空時自動用 `new OptTestExecutor()`。
+ `Arg` 只在 `{{nameof(OptTestExecutor.Arg)}}` 為空時才寫進去（`Opt.Arg ??= Arg`），
	即 `{{nameof(OptTestExecutor.Arg)}}` 優先。
+ 單節點版的重載只是把節點包成單元素列表後轉發給列表版。
]
""")]
public static partial class ExtnTreeTestExecutor{
	[Doc($$"""
#Sum[跑一棵子樹：不打印、不拋例外，結果由調用方自己處理。]
#Descr[
想自己出報告、或想按 `Summary.Failed` 自定義成敗判斷時用這個。
]
#Params([[執行器，即 `this`],
	[子樹的根節點，不能為空引用],
	[傳給用例的參數；只在 `{{nameof(OptTestExecutor.Arg)}}` 為空時才生效],
	[執行選項；傳空則用默認值]])
#Rtn[本次執行的匯總；有失敗時也只返回匯總、不拋]
""")]
	public static partial Task<TestRunSummary> RunTests(
		this ITestExecutor z
		,ITestNode Root
		,obj? Arg = default
		,OptTestExecutor? Opt = default
	);

	[Doc($$"""
#Sum[跑一棵子樹，跑完按默認格式打印匯總。]
#Descr[
本重載只是把單節點包成單元素列表後轉發給列表版，
打印格式與拋例外邏輯都在列表版（見 `{{nameof(WriteSummary)}}`）。
]
#Params([[執行器，即 `this`],
	[子樹的根節點，不能為空引用],
	[傳給用例的參數；只在 `{{nameof(OptTestExecutor.Arg)}}` 為空時才生效],
	[執行選項；傳空則用默認值],
	[為真時，有失敗則拋 `{{nameof(TestRunFailedException)}}`；默認 `false` 不拋]])
#Rtn[本次執行的匯總；`ThrowOnFailed` 為真且有失敗時改為拋例外、不返回]
""")]
	public static partial Task<TestRunSummary> RunEtPrint(
		this ITestExecutor z
		,ITestNode Root
		,obj? Arg = default
		,OptTestExecutor? Opt = default
		,bool ThrowOnFailed = false
	);

	[Doc($$"""
#Sum[跑一組節點：不打印、不拋例外。]
#Descr[
想一次跑多棵子樹、又要自己處理結果時用這個。
多個節點的結果合併成同一份匯總、按 `{{nameof(TestCaseRunResult.Order)}}` 排序。
]
#Params([[執行器，即 `this`],
	[要跑的根節點列表，不能為空引用],
	[傳給用例的參數；只在 `{{nameof(OptTestExecutor.Arg)}}` 為空時才生效],
	[執行選項；傳空則用默認值]])
#Rtn[本次執行的匯總；有失敗時也只返回匯總、不拋]
""")]
	public static partial Task<TestRunSummary> RunTests(
		this ITestExecutor z
		,IList<ITestNode> Nodes
		,obj? Arg = default
		,OptTestExecutor? Opt = default
	);

	[Doc($$"""
#Sum[跑一組節點，跑完打印匯總，可選在有失敗時拋例外。]
#Descr[
*打印格式*（`{{nameof(WriteSummary)}}`）：
先逐條打印失敗用例（測試類／用例名／耗時／用例路徑／異常全文），
再打印匯總（總數、通過、失敗、耗時）。
*通過的用例不逐條打印*，
免得全綠時刷屏。

*`ThrowOnFailed` 的語義*：
+ 為假（默認）：就算全部失敗也正常返回匯總。
+ 為真且 `Summary.Failed > 0`：*打印完之後*才拋 `{{nameof(TestRunFailedException)}}`，
	異常裏帶着同一份匯總（`{{nameof(TestRunFailedException.Summary)}}`），
	消息即「總數／通過／失敗」。

默認不拋是有意的：
本庫只負責跑與報告，
*進程的成敗應由入口自己判斷*
（入口可能只要求某些子樹必須全過）。
]
#Params([[執行器，即 `this`],
	[要跑的根節點列表，不能為空引用],
	[傳給用例的參數；只在 `{{nameof(OptTestExecutor.Arg)}}` 為空時才生效],
	[執行選項；傳空則用默認值],
	[為真時，有失敗則拋 `{{nameof(TestRunFailedException)}}`；默認 `false` 不拋]])
#Rtn[本次執行的匯總；`ThrowOnFailed` 為真且有失敗時改為拋例外、不返回]
""")]
	public static partial Task<TestRunSummary> RunEtPrint(
		this ITestExecutor z
		,IList<ITestNode> Nodes
		,obj? Arg = default
		,OptTestExecutor? Opt = default
		,bool ThrowOnFailed = false
	);

	[Doc($$"""
#Sum[打印失敗用例與匯總。]
#Descr[
*只逐條打印失敗的用例*，通過的不打印；
失敗那幾條按 `{{nameof(TestCaseRunResult.Order)}}` 排序輸出，
所以輸出順序與樹的先序一致、可穩定對比。

每條失敗打三行：
`[失敗] 測試類短名/用例名 （耗時）`、
縮進的用例路徑、
以及異常的 `ToString()` 全文（深紅）。

匯總部分：
`[CsTreeTest]` 標題，
`總數`、`通過`（有通過時標綠）、`失敗`（有失敗時標紅）、`耗時`。
`TesterType` 為空時測試類名顯示為 `UnknownTester`。
]
#Params([[要打印的匯總]])
#Rtn[無；直接寫到控制台]
""")]
	private static partial void WriteSummary(TestRunSummary Summary);

	[Doc($$"""
#Sum[用指定顏色寫一段文字，寫完恢復原顏色。]
#Descr[
不改調用方的顏色設置：
先存下 `Console.ForegroundColor`，寫完再還原。
]
#Params([[要寫的文字],
	[前景色]])
#Rtn[無]
""")]
	private static partial void WriteWithColor(str Text, ConsoleColor Color);

	[Doc($$"""
#Sum[用指定顏色寫一行文字，寫完恢復原顏色。]
#Descr[
與 `{{nameof(WriteWithColor)}}` 同，只是多一個換行；
異常全文用本方法輸出。
]
#Params([[要寫的文字],
	[前景色]])
#Rtn[無]
""")]
	private static partial void WriteLineWithColor(str Text, ConsoleColor Color);
}
