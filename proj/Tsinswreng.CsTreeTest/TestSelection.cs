using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：TestSelection 的成員宣告；實現見 TestSelection.Impl.cs。
[Doc($$"""
#Sum[一批已選中的用例。篩選與執行之間的交接物。]

#Descr[
職責邊界：
篩選只負責把用例挑出來（`{{nameof(TestFilter)}}`），
執行只負責跑（`{{nameof(TreeTestExecutor)}}`），
兩者靠本類型銜接。
所以可以「先 `{{nameof(TestFilter.Select)}}()` 拿到選集，
看一眼或改一改，
再決定甚麼時候 `{{nameof(TestSelection.Run)}}`」。

*`{{nameof(Nodes)}}` 仍在原樹上*：
挑選不從原樹摘除節點，
以免影響後續再篩選。
執行時另造一棵脫離原樹的臨時樹（見 `{{nameof(TestSelection.MkDetachedTree)}}`）。
]
""")]
public partial class TestSelection{
	[Doc($$"""
#Sum[選中的用例節點。]
#Descr[
節點仍留在原樹上，
不從原樹摘除，
以免影響後續再篩選。

列表由調用方持有，
增刪它會直接影響 `{{nameof(Count)}}` 與 `{{nameof(TestSelection.Run)}}` 的範圍。
]
""")]
	public IList<ITestNode> Nodes{get;}

	[Doc($$"""
#Sum[所屬的測試管理器。]
#Descr[
本類型自己不查索引，
需要索引的場合（例如調用方自行再篩）從這裏取。
]
""")]
	public ITestMgr Mgr{get;}

	[Doc($$"""
#Sum[造一個選集。]
#Descr[
一般不直接 new，
用 `{{nameof(TestFilter.Select)}}()` 造。

本類型不複製 `{{nameof(Nodes)}}`，
之後改那個列表等於改本選集。
]
#Params([[所屬測試管理器，不能為空],
	[選中的用例節點，不能為空；可以傳空列表表示一條都沒選中]])
""")]
	public partial TestSelection(ITestMgr Mgr, IList<ITestNode> Nodes);

	[Doc(@$"#Sum[命中的用例條數，即 `{nameof(Nodes)}` 的條數。]")]
	public partial i32 Count{get;}

	[Doc($$"""
#Sum[把命中的用例打印出來，不執行。]
#Descr[
用在不確定篩選條件寫得對不對、
想先看一眼再決定跑不跑的場合。

用在 `{{nameof(TestSelection.PrintHits)}}` 之後：
那一步判斷條件，
這一步確認最終選集。
]
""")]
	public partial void PrintHits();

	[Doc($$"""
#Sum[取命中的用例本身的元數據，供調用方自行查看。]
#Descr[
惰性枚舉（`yield return`），
一次調用只算一遍，
不要多次消費同一個返回值。

只取 `Data` 是 `{{nameof(ITestCase)}}` 的節點。
]
#Rtn[用例元數據序列，次序與 `{{nameof(Nodes)}}` 相同]
""")]
	public partial IEnumerable<ITestCase> Cases();

	[Doc($$"""
#Sum[把選中的用例拼成一棵可直接執行、脫離原樹的臨時樹。]
#Descr[
*為甚麼要另造樹*：
挑選結果在樹上可能四散、拼不回單一根節點，
而執行器要的是節點。

*造出來的樹是扁平的*：
新建一個脫離原樹的根（唯一名 `SelectedCases`），
把命中的用例按 `{{nameof(Nodes)}}` 的次序逐個掛成一級子節點。
代價是原樹上那些分組節點的
*順序與並行設定不再適用*，
這是已知取捨；
需要順序或並行時，
由調用方在 `{{nameof(OptTestExecutor)}}` 上指定（見 `{{nameof(MakeOpt)}}`）。

*用例會複製一份*：
`{{nameof(TestCase)}}` 帶着可變的列表欄位，
複製一次可避免臨時樹與原樹共享同一個用例物件。
]
#Rtn[臨時樹的根；唯一名為 `SelectedCases`，其子節點即選中的用例]
""")]
	public partial ITestNode MkDetachedTree();

	[Doc($$"""
#Sum[執行選中的用例，返回匯總但不打印。]
#Descr[
與 `{{nameof(ExtnTreeTestExecutor.RunEtPrint)}}` 的差別只有*打印*：
本方法把匯總交回調用方，
適合要自己出報告、或要把多次執行的結果併起來的場合。

執行的是 `{{nameof(TestSelection.MkDetachedTree)}}()` 造出來的扁平臨時樹。

*有失敗也不拋例外*：
本庫默認只跑與報告，
進程的成敗由入口自己判斷。

每次調用都重新造一次臨時樹與執行器，
因此同一個選集可以重複跑。
]
#Params([[執行選項；為空時取「不並行」的默認選項，見 `{{nameof(MakeOpt)}}`],
	[傳給用例函數的參數；`{{nameof(OptTestExecutor.Arg)}}` 已有值時以 `{{nameof(OptTestExecutor.Arg)}}` 為準]])
#Rtn[本次執行的匯總統計]
""")]
	public partial Task<TestRunSummary> Run(OptTestExecutor? Opt = null, obj? Arg = null);

	[Doc($$"""
#Sum[執行選中的用例，並按默認格式打印匯總。]
#Descr[
與 `{{nameof(TestSelection.Run)}}` 走同一條執行路徑，
只在跑完之後多打印一次匯總，
適合寫在測試入口腳本裏直接看結果。

要自己處理結果、或不想讓控制台被測試輸出淹掉時用 `{{nameof(TestSelection.Run)}}`。
]
#Params([[執行選項；為空時取「不並行」的默認選項，見 `{{nameof(MakeOpt)}}`],
	[傳給用例函數的參數；`{{nameof(OptTestExecutor.Arg)}}` 已有值時以 `{{nameof(OptTestExecutor.Arg)}}` 為準]])
#Rtn[本次執行的匯總統計]
""")]
	public partial Task<TestRunSummary> RunEtPrint(OptTestExecutor? Opt = null, obj? Arg = null);

	[Doc($$"""
#Sum[組出執行選項；選集預設不並行。]
#Descr[
*選集預設 `IsParallel = false`*：
挑出來的用例原本散在樹上，
彼此之間是否獨立、
原樹的分組順序是否被破壞，
都難以自動判斷；
而且臨時樹是平的（見 `{{nameof(TestSelection.MkDetachedTree)}}`），
原樹的並行設定已經丟失。
因此預設求穩。
確認彼此獨立之後，
調用方可以顯式傳一個 `IsParallel = true` 的選項打開並行。

*會就地修改傳進來的 `Opt`*（只寫 `Arg`）：
要復用同一個 `Opt` 就自己傳副本。
]
#Params([[執行選項；為空時新建一個並行關閉的],
	[參數；只在 `{{nameof(OptTestExecutor.Arg)}}` 為空時才寫進去，不覆蓋已有值]])
#Rtn[可直接交給執行器的選項]
""")]
	private static partial OptTestExecutor MakeOpt(OptTestExecutor? Opt, obj? Arg);
}
