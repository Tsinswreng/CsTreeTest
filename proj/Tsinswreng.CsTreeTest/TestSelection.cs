using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：TestSelection 的成員宣告；實現見 TestSelection.Impl.cs。
[Doc($$"""
一批已選中的用例，篩選與執行之間的交接物。

`{{nameof(Nodes)}}` 仍留在原樹上；執行時另造一棵脫離原樹的扁平臨時樹。
""")]
public partial class TestSelection{
	[Doc($$"""
選中的用例節點，仍在原樹上；改這個列表等於改本選集。
""")]
	public IList<ITestNode> Nodes{get;}

	[Doc($$"""
所屬測試管理器；本類型自己不查索引。
""")]
	public ITestMgr Mgr{get;}

	[Doc($$"""
一般不直接 new，用 `{{nameof(TestFilter.Select)}}` 造。
#Prm[{{nameof(Nodes)}}][不複製，之後改它等於改本選集]
""")]
	public partial TestSelection(ITestMgr Mgr, IList<ITestNode> Nodes);

	[Doc($$"""
命中的用例條數。
""")]
	public partial i32 Count{get;}

	[Doc($$"""
打印選中的用例路徑，不執行。
""")]
	public partial void PrintHits();

	[Doc($$"""
惰性枚舉選中的用例元數據，次序與 `{{nameof(Nodes)}}` 相同。
""")]
	public partial IEnumerable<ITestCase> Cases();

	[Doc($$"""
拼成一棵脫離原樹、可直接執行的扁平臨時樹。

根名 `SelectedCases`，命中的用例按 `{{nameof(Nodes)}}` 的次序掛成一級子節點；用例會複製一份。
原樹的分組順序與並行設定因此丟失——要順序或並行，在 `{{nameof(OptTestExecutor)}}` 上指定。

#Rtn[臨時樹的根]
""")]
	public partial ITestNode MkDetachedTree();

	[Doc($$"""
跑選中的用例，返回匯總但不打印；有失敗也不拋例外。

#Prm[{{nameof(Opt)}}][為空時用「不並行」的默認選項]
#Prm[{{nameof(Arg)}}][只在 `{{nameof(OptTestExecutor.Arg)}}` 為空時才寫進去]
#Rtn[本次執行的匯總]
""")]
	public partial Task<TestRunSummary> Run(OptTestExecutor? Opt = null, obj? Arg = null);

	[Doc($$"""
同上，跑完按默認格式打印匯總。

#Prm[{{nameof(Opt)}}][為空時用「不並行」的默認選項]
#Prm[{{nameof(Arg)}}][只在 `{{nameof(OptTestExecutor.Arg)}}` 為空時才寫進去]
#Rtn[本次執行的匯總]
""")]
	public partial Task<TestRunSummary> RunEtPrint(OptTestExecutor? Opt = null, obj? Arg = null);

	[Doc($$"""
組出執行選項；選集預設 `IsParallel = false`。

會就地修改傳進來的 `{{nameof(Opt)}}`（只寫 `{{nameof(OptTestExecutor.Arg)}}`），要復用就自己傳副本。

#Rtn[可直接交給執行器的選項]
""")]
	private static partial OptTestExecutor MakeOpt(OptTestExecutor? Opt, obj? Arg);
}
