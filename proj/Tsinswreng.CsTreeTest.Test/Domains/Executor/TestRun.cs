using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Executor;

// 分片聲明文件：本片測試 TreeTestExecutor.Run；實現見 TestRun.Impl.cs。
[Doc($$"""
#Sum[本檔只測 TreeTestExecutor.Run：]
#Descr[
依序與並行兩種模式、
參數傳遞、
失敗記錄、
結果匯總。
]
""")]
public partial class TestExecutor{
	[Doc($$"""
#Sum[往節點下掛一條用例，用例體由 `Fn` 給。]
#Params([[用例掛上去的父節點],[用例名，也就是節點名],[用例函數本體]])
""")]
	private static partial void AddCase(ITestNode Parent, str UniqName, FnTest Fn);

	[Doc($$"""
#Sum[建一棵首層帶 若干個用例的臨時樹並執行。]
#Descr[
每個用例把執行的次序記進 `Seen`，
用例之間以奇偶交錯的延時放大「亂序」的風險：
若執行器真的把次序弄亂，這裡會露餡。
]
#Params([[用例條數],[收集執行次序的列表，用例會往裏追加],[執行選項；傳空則用不並行的默認選項]])
""")]
	private static partial Task RunScratchTree(i32 Count, IList<i32> Seen, OptTestExecutor? Opt);

	[Doc($$"""
#Sum[把 Run 的用例掛到節點上。]
""")]
	public partial void RegisterRun(ITestNode Node);

	[Doc($$"""
#Sum[依序模式保持聲明次序。]
""")]
	public partial Task<nil> SequentialModeKeepsDeclaredOrder(obj? O);

	[Doc($$"""
#Sum[並行模式確實同時跑多條用例。]
""")]
	public partial Task<nil> ParallelModeRunsCasesConcurrently(obj? O);

	[Doc($$"""
#Sum[Opt.Arg 傳給每條用例。]
""")]
	public partial Task<nil> OptArgIsPassedToEachCase(obj? O);

	[Doc($$"""
#Sum[失敗用例被記下且不中斷其餘用例。]
""")]
	public partial Task<nil> FailedCaseIsRecordedAndDoesNotStopOthers(obj? O);

	[Doc($$"""
#Sum[全部通過時統計正確。]
""")]
	public partial Task<nil> AllPassedSummaryIsCorrect(obj? O);
}