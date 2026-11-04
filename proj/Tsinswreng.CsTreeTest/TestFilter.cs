using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：TestFilter 的成員宣告；實現見 TestFilter.Impl.cs。
[Doc($$"""
按條件把要跑的那批用例挑出來；多個條件之間是 AND。

走管理器索引，不重掃全樹；範圍是硬邊界，結果按樹的先序穩定排序。
要「或」就用 `{{nameof(Where)}}` 把或關係寫進一個謂詞。
```cs
var Hits = Mgr.{{nameof(TestMgrExtn.Filter)}}().{{nameof(TesteeType)}}<ISvcUser>().{{nameof(TesteeFn)}}(nameof(ISvcUser.Login)).{{nameof(Select)}}();
Hits.{{nameof(PrintHits)}}();
await Hits.{{nameof(TestSelection.RunEtPrint)}}();
```
""")]
public partial class TestFilter{
	[Doc($$"""
篩選所依據的測試管理器；構造時固定。
""")]
	private readonly ITestMgr Mgr;

	[Doc($$"""
篩選範圍；範圍外的候選一律剔掉。
""")]
	private readonly ITestNode Scope;

	[Doc($$"""
測試類條件；為空表示不限制，同時是候選集的來源。
""")]
	private Type? _TesterType;

	[Doc($$"""
被測類型條件；為空表示不限制。
""")]
	private Type? _TesteeType;

	[Doc($$"""
被測函數名條件；為空表示不限制。
""")]
	private str? _TesteeFnName;

	[Doc($$"""
用例名包含某段文字；不分大小寫，用不上索引。
""")]
	private str? _NameContains;

	[Doc($$"""
自訂條件，全部要通過；謂詞只拿到用例元數據，拿不到節點。
""")]
	private readonly List<Func<ITestCase, bool>> _ExtraConditions = [];

	[Doc($$"""
一般不直接 new，用 `{{nameof(TestMgrExtn.Filter)}}` 造。
#Prm[{{nameof(Scope)}}][為空時取整棵樹的根]
""")]
	public partial TestFilter(ITestMgr Mgr, ITestNode? Scope = null);

	[Doc($$"""
收窄範圍，返回新篩選器；原篩選器與它的條件不受影響。
#Prm[{{nameof(Scope)}}][子樹的根]#Rtn[新篩選器，已設條件原樣帶過去]
""")]
	public partial TestFilter Under(ITestNode Scope);

	[Doc($$"""
限定測試類；重複調用覆蓋前值，傳 `null` 取消本條件。
#Rtn[本篩選器，便於鍊式調用]
""")]
	public partial TestFilter TesterType(Type TesterType);

	[Doc($$"""
同上，類型由泛型給。
#TPrm[{{nameof(TTester)}}][測試類類型]#Rtn[本篩選器]
""")]
	public partial TestFilter TesterType<TTester>();

	[Doc($$"""
限定被測類型；重複調用覆蓋前值。
#Rtn[本篩選器，便於鍊式調用]
""")]
	public partial TestFilter TesteeType(Type TesteeType);

	[Doc($$"""
同上，類型由泛型給。
#TPrm[{{nameof(TTestee)}}][被測類型]#Rtn[本篩選器]
""")]
	public partial TestFilter TesteeType<TTestee>();

	[Doc($$"""
限定被測函數名；必須用 `nameof` 賦值。
#Rtn[本篩選器，便於鍊式調用]
""")]
	public partial TestFilter TesteeFn(str TesteeFnName);

	[Doc($$"""
限定用例名包含某段文字；不分大小寫，比的是完整用例名（含前綴）。
#Rtn[本篩選器，便於鍊式調用]
""")]
	public partial TestFilter NameContains(str Fragment);

	[Doc($$"""
再加一個自訂條件（仍是 AND）。

要「或」或「排除」就寫進這一個謂詞裏。

#Prm[{{nameof(Predicate)}}][返回 `true` 表示通過本條件，不能為空]
#Rtn[本篩選器，便於鍊式調用]
""")]
	public partial TestFilter Where(Func<ITestCase, bool> Predicate);

	[Doc($$"""
取出命中的用例節點，按樹的先序排好。

#Rtn[新建的列表，與索引無關，可自由改動]
""")]
	public partial IList<ITestNode> Apply();

	[Doc($$"""
把命中結果包成可打印、可執行的選集。
#Rtn[選集]
""")]
	public partial TestSelection Select();

	[Doc($$"""
只打印命中了哪些用例，不執行。
""")]
	public partial void PrintHits();

	[Doc($$"""
按索引挑候選，盡量少掃節點。

有測試類條件就以它的子樹為底；否則取被測函數名/類型那組，兩者都有就求交集；
都用不上索引時才遍歷 `Scope`。
候選最後一律過 `{{nameof(Restrict)}}`，保證仍受範圍限制。

#Rtn[候選節點，可能含不相關的，交給逐條匹配篩掉]
""")]
	private partial IList<ITestNode> PickCandidates();

	[Doc($$"""
把候選限制在 `Scope` 内；按引用比對。
#Rtn[落在範圍内的候選]
""")]
	private partial IList<ITestNode> Restrict(IList<ITestNode> Nodes);

	[Doc($$"""
求交集，保留 `A` 的次序。
#Rtn[兩者共有的節點]
""")]
	private static partial IList<ITestNode> Intersect(IList<ITestNode> A, IList<ITestNode> B);

	[Doc($$"""
判斷一條用例是否滿足全部條件；未設的條件不參與。
#Rtn[是否全部滿足]
""")]
	private partial bool Match(ITestCase Case);
}
