using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：TreeTestExecutorExtn 擴展方法的宣告；實現見 TreeTestExecutorExtn.Impl.cs。
[Doc($$"""
執行器的便捷入口。單節點版只是把節點包成單元素列表後轉發給列表版。

`Nodes` / `{{nameof(ITestNode)}}` 為空引用時拋 `ArgumentNullException`。
`Arg` 只在 `{{nameof(OptTestExecutor.Arg)}}` 為空時才寫進去，即 `{{nameof(OptTestExecutor.Arg)}}` 優先。
""")]
public static partial class TreeTestExecutorExtn{
	[Doc($$"""
跑一棵子樹：不打印、不拋例外。
#Rtn[匯總；有失敗時也只返回匯總]
""")]
	public static partial Task<TestRunSummary> RunTests(
		this ITestExecutor z
		,ITestNode Root
		,obj? Arg = default
		,OptTestExecutor? Opt = default
	);

	[Doc($$"""
跑一棵子樹，跑完按默認格式打印匯總。
#Prm[{{nameof(ThrowOnFailed)}}][為真且有失敗時拋 `{{nameof(TestRunFailedException)}}`，默認不拋]#Rtn[匯總]
""")]
	public static partial Task<TestRunSummary> RunEtPrint(
		this ITestExecutor z
		,ITestNode Root
		,obj? Arg = default
		,OptTestExecutor? Opt = default
		,bool ThrowOnFailed = false
	);

	[Doc($$"""
跑一組節點：不打印、不拋例外；各節點的結果併成一份匯總。
#Rtn[匯總；有失敗時也只返回匯總]
""")]
	public static partial Task<TestRunSummary> RunTests(
		this ITestExecutor z
		,IList<ITestNode> Nodes
		,obj? Arg = default
		,OptTestExecutor? Opt = default
	);

	[Doc($$"""
跑一組節點，跑完打印匯總，可選在有失敗時拋例外。

只逐條打印失敗的用例，通過的不打印。
`{{nameof(ThrowOnFailed)}}` 為真且 `Summary.Failed > 0` 時，*打印完之後*才拋 `{{nameof(TestRunFailedException)}}`。
默認不拋：本庫只管跑與報告，進程成敗由入口自己判斷。

#Prm[{{nameof(ThrowOnFailed)}}][為真時有失敗就拋例外，默認不拋]
#Rtn[匯總；`{{nameof(ThrowOnFailed)}}` 為真且有失敗時改為拋例外]
""")]
	public static partial Task<TestRunSummary> RunEtPrint(
		this ITestExecutor z
		,IList<ITestNode> Nodes
		,obj? Arg = default
		,OptTestExecutor? Opt = default
		,bool ThrowOnFailed = false
	);

	[Doc($$"""
打印失敗用例與匯總；通過的不逐條打印。
""")]
	private static partial void WriteSummary(TestRunSummary Summary);

	[Doc($$"""
用指定顏色寫一段文字，寫完恢復原顏色。
""")]
	private static partial void WriteWithColor(str Text, ConsoleColor Color);

	[Doc($$"""
同上，多一個換行。
""")]
	private static partial void WriteLineWithColor(str Text, ConsoleColor Color);
}
