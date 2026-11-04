using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：TestMgr 的成員宣告；實現見 TestMgr.Impl.cs。
[Doc($$"""
#Sum[測試管理器的默認實現：子類只需在 `SetupTree` 裏聲明註冊哪些測試類。]

#Descr[
各測試項目在自己的子類裏重寫 `SetupTree`，
在其中聲明要註冊哪些測試類、
以及樹根的順序與並行設定。

```cs
public class MyProj1TestMgr : TestMgr{
	public static MyProj1TestMgr Inst = new();

	protected override void SetupTree(){
		RegisterTester<TestCalculator>();                  // 直接掛在根下
		RegisterTesterIntoBranch<TestSvcUser>("SvcUser");  // 歸到領域分組下
		RegisterSubMgr(OtherProjectTestMgr.Inst);          // 收編下級管理器
	}
}
```

*生命週期分兩段*：
`SetupTree` 階段只登記（測試類類型、分組名、下級管理器），
`{{nameof(TestMgr.Init)}}` 階段才實例化、掛樹、建索引。
兩段分開是為了讓子類在 `SetupTree` 裏可以任意安排聲明順序，
而實例化時機總在依賴來源就緒之後。

入口三步的順序不能顛倒：
先備好全局依賴來源、
再 `{{nameof(TestMgr.Init)}}()`、
最後交給執行器跑。
]
""")]
public abstract partial class TestMgr : ITestMgr{
	[Doc($$"""
#Sum[一個待註冊的測試類：記着類型與「怎麼 new 出來」。]
#Descr[
`MkTester` 之所以是委託而不是類型，
是因為 AOT 下不能用反射實例化；
由 `{{nameof(TestMgr.RegisterTester)}}<T>` 傳入 `() => new T()` 即可。

這是內部機制，不作為公開 API：
測試類一律由管理器 `new` 出來，
依賴由測試項目自己的全局依賴來源提供。
]
#Params([[測試類類型；寫進測試類節點與索引靠它],
	[實例化動作；`{{nameof(TestMgr.Init)}}` 時才調用],
	[測試類節點的節點名；為空時由 `{{nameof(ITestNodeExtn.NewTesterChild)}}` 取測試類短名],
	[領域分組名；為空表示直接掛在根下]])
""")]
	private sealed record PendingTester(
		Type TesterType
		,Func<ITester> MkTester
		,str? NodeName
		,str? BranchName
	);

	[Doc($$"""
#Sum[待註冊的測試類隊列。]
#Descr[
登記時不實例化，
實例化統一放在 `{{nameof(TestMgr.Init)}}` 裏，
這樣子類可以任意安排聲明順序。
]
""")]
	private readonly List<PendingTester> _pendingTesters = [];

	[Doc($$"""
#Sum[待收編的下級管理器隊列。]
#Descr[
收編也是登記：
`{{nameof(TestMgr.RegisterSubMgr)}}` 只把下級管理器記在這裏，
真正的掛樹與索引合併發生在本管理器的 `{{nameof(TestMgr.Init)}}` 裏。

*為甚麼不能立刻掛*：
本管理器的 `{{nameof(TestMgr.Init)}}` 會重建索引，
立刻併進來的索引條目會被那次重建清掉。
]
""")]
	private readonly List<ITestMgr> _pendingSubMgrs = [];

	[Doc($$"""
#Sum[保護登記與初始化的互斥鎖。]
#Descr[
`_pendingTesters`、
`_pendingSubMgrs`、
`_initialized` 三者都在本鎖內讀寫，
避免一邊登記一邊初始化。

本鎖只護住這三個字段；
測試樹與索引的其它訪問不在它的保護範圍內。
]
""")]
	private readonly object _sync = new();

	[Doc($$"""
#Sum[是否已經初始化過。]
#Descr[
置真之後一律禁止再登記測試類或下級管理器，
重複調用 `{{nameof(TestMgr.Init)}}` 也會拋例外。

*必須在 `SetupTree()` 返回之後才置真*：
`SetupTree` 本身就是子類登記的地方，
提前置真會讓子類在 `SetupTree` 裏調 `{{nameof(TestMgr.RegisterTester)}}`
被 `{{nameof(EnsureNotInitialized)}}` 攔住，
於是所有測試類都註冊不進去。
]
""")]
	private bool _initialized = false;

	[Doc($$"""
#Sum[測試樹的根。]
#Descr[
唯一名固定為 `Tests`（`{{nameof(TestMgr.Init)}}` 時會再寫一次），
使失敗輸出的路徑可讀。

`protected set` 供子類換成別的根實現；
換根要在 `{{nameof(TestMgr.Init)}}` 之前做，否則索引與樹會對不上。
]
""")]
	public ITestNode Root{get;protected set;} = new TestNode{UniqName = "Tests"};

	[Doc($$"""
#Sum[索引；`{{nameof(TestMgr.Init)}}` 時整體重建。]
#Descr[
`{{nameof(TestMgr.Init)}}` 會丟掉舊索引重新建，
所以任何想在初始化之後仍然有效的索引改動，
都必須發生在 `{{nameof(TestMgr.Init)}}` 之內或之後
（`{{nameof(TestMgr.RegisterSubMgr)}}` 的延遲合併就是為此）。
]
""")]
	public TestIndex Index{get;protected set;} = new();

	[Doc($$"""
#Sum[子類在此聲明要註冊的測試類、領域分組與下級管理器。]
#Descr[
由 `{{nameof(TestMgr.Init)}}` 調用，只調一次。

本方法內*只做聲明*：
不要在這裏跑用例，
也不要直接往 `{{nameof(TestMgr.Root)}}` 掛節點——
掛載由 `{{nameof(TestMgr.Init)}}` 統一安排，
否則掛上去的東西不會進索引。

在本方法內可以放心調用
`{{nameof(TestMgr.RegisterTester)}}`、
`{{nameof(TestMgr.RegisterTesterIntoBranch)}}`、
`{{nameof(TestMgr.RegisterSubMgr)}}`，
因為 `_initialized` 要到本方法返回之後才置真。
]
""")]
	protected abstract void SetupTree();

	[Doc($$"""
#Sum[註冊一個測試類，直接掛在根下。]
#Descr[
*測試類由測試管理器自行 `new`*，
不經過任何依賴容器，
因此測試類*不得有帶參數的建構子*
（泛型約束也是 `new()`，編譯期就會擋下）；
它需要的依賴應在自己的欄位初始化時
向測試項目自己的全局依賴來源取——
慣例是各測試項目自備一個靜態 `Di` 類 + `GetRSvc<T>()`。
框架對依賴注入零概念。

只登記、不實例化；
實例化與掛樹發生在 `{{nameof(TestMgr.Init)}}`。

必須在 `{{nameof(TestMgr.Init)}}` 之前調用，之後調用拋例外。

例：
```cs
RegisterTester<TestCalculator>();
RegisterTester<TestCalculator>("Calculator");  // 顯式指定節點名
```
]
#TParams([測試類類型；必須實現 `{{nameof(ITester)}}` 且有無參建構子])
#Params([[測試類節點的節點名；為空時取測試類短名]])
""")]
	public partial void RegisterTester<T>(str? NodeName = null) where T:ITester,new();

	[Doc($$"""
#Sum[註冊一個測試類，並把它歸到某個領域分組之下。]
#Descr[
領域分組會自動建在根下（`{{nameof(TestMgr.Init)}}` 時統一建），
不必先手工調 `{{nameof(TestMgr.AddBranch)}}`。

同名分組只會建一個節點：
多個測試類聲明同一個分組時它們共用那個節點。

對測試類的要求與 `{{nameof(TestMgr.RegisterTester)}}` 相同：
無參建構子、依賴自取。

只登記、不實例化。
]
#TParams([測試類類型；必須實現 `{{nameof(ITester)}}` 且有無參建構子])
#Params([[領域分組名，不能為空],
	[測試類節點的節點名；為空時取測試類短名]])
""")]
	public partial void RegisterTesterIntoBranch<T>(
		str BranchName, str? NodeName = null
	)
		where T:ITester,new();

	[Doc($$"""
#Sum[登記一個待掛的測試類：只入隊，不實例化。]
#Descr[
`{{nameof(TestMgr.RegisterTester)}}` 與 `{{nameof(TestMgr.RegisterTesterIntoBranch)}}` 的唯一出口，
因此「初始化之後禁止再登記」這條約束只在這裏把一次。
]
#Params([[測試類類型，不能為空],
	[實例化動作，不能為空],
	[測試類節點的節點名],
	[領域分組名；為空表示掛在根下]])
""")]
	private partial void AddPendingTester(
		Type TesterType
		,Func<ITester> MkTester
		,str? NodeName
		,str? BranchName
	);

	[Doc($$"""
#Sum[初始化：重建索引、實例化測試類並掛樹、收編下級管理器。]
#Descr[
只允許調用一次。
具體步驟見實現內註釋。

*順序上的兩個要點*：
`_initialized` 要在 `SetupTree()` 返回之後才置真；
下級管理器要在測試類全部掛完之後才收編。
兩者錯了都會靜默丟東西（測試類註冊不進去、索引條目被清掉）。
]
#Rtn[測試樹的根]
""")]
	public partial ITestNode Init();

	[Doc($$"""
#Sum[在根下建一個領域分組節點；已存在同名分組時直接返回它。]
#Descr[
領域分組只作分組、不帶用例（`{{nameof(ITestNode.Data)}}` 為空），
因此不會進被測類型與被測函數名的索引。

*幂等*：
同名分組重複調用返回同一個節點，
使多個測試類可以歸到同一個分組而不會撞名。

可在 `{{nameof(TestMgr.Init)}}` 之前或之後調用；
分組是普通節點，不經過 `{{nameof(PendingTester)}}` 那套登記。
]
#Params([[分組名，不能為空；同時作為節點的唯一名]])
#Rtn[分組節點；已存在時是已有的那個]
""")]
	public partial ITestNode AddBranch(str BranchName);

	[Doc($$"""
#Sum[實例化一個測試類、掛節點、讓它註冊用例，最後把新子樹入索引。]
#Descr[
順序不能顛倒：
必須先有父節點（分組可能現建），
再有測試類節點，
再回調 `{{nameof(ITester.RegisterTestsInto)}}`，
最後才入索引——
否則索引裏會缺這批用例。

測試類節點的節點名為空時取測試類短名，
使失敗輸出的路徑能直接看出是哪個測試類。
]
#Params([[待掛的測試類]])
""")]
	private partial void MountTester(PendingTester Pending);

	[Doc($$"""
#Sum[登記一個待收編的下級管理器。]
#Descr[
只登記、不立刻掛樹。
*為甚麼*：本管理器的 `{{nameof(TestMgr.Init)}}` 會重建索引，
立刻併進來的索引條目會被那次重建清掉。

必須在本管理器 `{{nameof(TestMgr.Init)}}` 之前調用，
之後調用拋例外。
]
#Params([[要收編的下級管理器，不能為空]])
""")]
	public partial void RegisterSubMgr(ITestMgr SubMgr);

	[Doc($$"""
#Sum[把一個已登記的下級管理器整棵掛到本管理器的樹上。]
#Descr[
由 `{{nameof(TestMgr.Init)}}` 統一調用，時機在測試類掛完之後，
這樣合併出來的索引包含全部測試類的條目。

*為甚麼用新節點承接、並以子管理器類型名命名*：
子管理器原本的根只是為了讓它自己成樹而存在；
直接把它接到父樹上會讓兩棵樹共用同一個節點實例，
而且它的默認名也是 `Tests`，
與父根撞名會讓索引互相覆蓋。
因此新建一個以*子管理器類型名*命名的節點，
只把子根的設定（`{{nameof(ITestNode.Ordered)}}`、`{{nameof(ITestNode.IsParallelRecursive)}}`）與子節點搬過來。
同名子管理器只能收編一次，
第二次會因唯一名重複而拋例外。

搬子節點時父子兩邊的引用都要維護。
]
#Params([[已登記的下級管理器；本方法內會代它 `{{nameof(TestMgr.Init)}}`]])
""")]
	private partial void MountSubMgr(ITestMgr SubMgr);

	[Doc($$"""
#Sum[把子樹上每個節點連同它的元數據記進索引。]
#Descr[
以先序走整棵子樹（含根本身）。
重複登記同一個節點實例不會拋例外。
]
#Params([[子樹的根，含根本身]])
""")]
	private partial void IndexSubtree(ITestNode SubRoot);

	[Doc($$"""
#Sum[初始化之後拒絕再登記，直接拋例外。]
#Descr[
`{{nameof(AddPendingTester)}}` 與 `{{nameof(TestMgr.RegisterSubMgr)}}` 共用。
*不*用於 `{{nameof(TestMgr.Init)}}` 自身：
`{{nameof(TestMgr.Init)}}` 裏的 `SetupTree` 必須能登記，
所以 `_initialized` 是 `SetupTree` 返回之後才置真。
]
""")]
	private partial void EnsureNotInitialized();
}
