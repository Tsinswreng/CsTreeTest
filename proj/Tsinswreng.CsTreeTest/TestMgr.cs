using Tsinswreng.CsCore;


namespace Tsinswreng.CsTreeTest;

// 聲明文件：字段/屬性 + 構造函數/方法聲明（partial method）；實現見 TestMgr.Impl.cs。
[Doc($$"""
#Sum[測試索引：把測試樹上的節點按四種鍵各記一份，供查詢與篩選使用。]

#Descr[
*四組索引各自怎麼建、各有甚麼約束*：

+ `{{nameof(TestIndex.UniqName_TestNode)}}`（唯一名到節點，一對一）：
	節點名必須在同一棵樹內唯一；
	同名但*不是同一個節點實例*時，
	登記直接拋例外。
+ `{{nameof(TestIndex.TesterType_TestNode)}}`（測試類類型到節點，一對一）：
	一個測試類只註冊一個節點；
	同一個測試類被登記兩次（兩個不同實例）也拋例外。
+ `{{nameof(TestIndex.TesteeType_TestNode)}}`（被測類型到節點，一對多）：
	一個被測類的用例可能散在多個測試類下，
	所以值是列表。
+ `{{nameof(TestIndex.TesteeFnName_TestNode)}}`（被測函數名到節點，一對多）：
	理由同上。

*索引是查詢與篩選的唯一依據。*
樹上每掛一次東西（測試類、領域分組、下級管理器）之後都要同步更新索引，
否則查詢結果會與樹不一致：
查得到卻不在樹上、或樹上有卻查不到。

只有*帶用例的葉節點*會進被測類型與被測函數名這兩組索引；
分組節點的 `{{nameof(ITestNode.Data)}}` 為空，
因而不會進來。
]
""")]
public partial class TestIndex{
	[Doc($$"""
#Sum[唯一名到節點，一對一。]
#Descr[
每個鍵只對應一個節點。
唯一名為空的節點不進本組。
重複的鍵（不同節點實例）由 `Add` 攔下並拋例外。
]
""")]
	public IDictionary<str, ITestNode> UniqName_TestNode{get;set;}
		= new Dictionary<str, ITestNode>();

	[Doc($$"""
#Sum[測試類類型到節點，一對一。]
#Descr[
每個測試類只註冊一個節點。
沒有測試類歸屬的節點（如領域分組、子管理器的掛載節點）不進本組。
]
""")]
	public IDictionary<Type, ITestNode> TesterType_TestNode{get;set;}
		= new Dictionary<Type, ITestNode>();

	[Doc($$"""
#Sum[被測類型到節點，一對多。]
#Descr[
一對多：
一個被測類的用例可能散在多個測試類下。
列表內不重複記同一個節點。
]
""")]
	public IDictionary<Type, IList<ITestNode>> TesteeType_TestNode{get;set;}
		= new Dictionary<Type, IList<ITestNode>>();

	[Doc($$"""
#Sum[被測函數名到節點，一對多。]
#Descr[
一對多。
鍵是函數名（不是方法簽名），
因此*不同類型的同名方法會撞在一起*，
這是按函數名篩選時已知的粒度限制。
]
""")]
	public IDictionary<str, IList<ITestNode>> TesteeFnName_TestNode{get;set;}
		= new Dictionary<str, IList<ITestNode>>();

	[Doc($$"""
#Sum[把一個節點連同它的用例元數據記進索引，測試類類型由節點自身取。]
#Descr[
節點實現了 `{{nameof(ITesterNode)}}` 時取其 `TesterType`，
否則測試類類型記為空——
即*測試類這組不登記*，
唯一名與用例元數據兩組照常登記。
]
#Params([[要入索引的節點]])
""")]
	public partial void Add(ITestNode Node);

	[Doc($$"""
#Sum[把一個節點連同它的用例元數據記進索引，測試類類型由參數顯式給。]
#Descr[
唯一名或測試類的鍵已存在、
且對應的不是*同一個節點實例*時拋 `InvalidOperationException`：
前者意味着兩個節點撞名，
後者意味着同一個測試類被註冊了兩次。

重複登記*同一個*節點實例是允許的（覆蓋自己），
子樹入索引時就會走到這條路。

被測函數名為空白的條目會被跳過（記了也查不到）。
]
#Params([[要入索引的節點],
	[測試類類型；為空表示這個節點不屬於任何測試類，只登記唯一名]])
""")]
	public partial void Add(ITestNode Node, Type? TesterType);

	[Doc($$"""
#Sum[把另一個索引的全部條目併入本索引。]
#Descr[
收編下級測試管理器時用，
否則上層按測試類或按被測函數名查不到下級的用例。

`SkipUniqName` 用來跳過「已經不在樹上」的那個節點條目，
一般是下級管理器自己的根：
它已不在掛好之後的樹上，
而且默認名也是 `Tests`，
併進來會與父根互相覆蓋。

*為甚麼是「跳過」而不是「先併入再刪」*：
本索引裏可能已經有同名的合法條目（父根就叫 `Tests`），
先併入再刪會把那個合法條目一起誤刪；
索引也沒有 `Remove` 這類接口，
所以「跳過」是唯一安全的路子。

一對多的兩組按*追加*合併，
因此不會覆蓋已有的合法條目；
測試類那組是一對一、直接覆蓋。
]
#Params([[要併入的索引，不能為空],
	[合併唯一名這組時要跳過的鍵；為空時不跳過]])
""")]
	public partial void MergeFrom(TestIndex Other, str? SkipUniqName = null);

	[Doc($$"""
#Sum[按唯一名取節點。]
#Descr[
取不到返回空，
由調用方決定是拋例外還是當作沒有——
`{{nameof(ExtnTestMgr.GetNodeByName)}}` 選的是拋例外。
]
#Params([[節點唯一名]])
#Rtn[節點；查不到時為空]
""")]
	public partial ITestNode? FindByName(str UniqName);

	[Doc($$"""
#Sum[按測試類取節點。]
#Descr[
只適用於已登記的測試類；
該測試類沒註冊過時返回空。
]
#Params([[測試類類型]])
#Rtn[測試類節點；查不到時為空]
""")]
	public partial ITestNode? FindByTesterType(Type TesterType);

	[Doc($$"""
#Sum[按被測類型取節點。]
#Descr[
取不到時返回*新建的空列表*而不是 `null`，
調用方可直接遍歷。

取到時返回的是*索引內部的列表本身，不是副本*，
請勿在外部增刪它，
否則索引會與樹不一致。
]
#Params([[被測類型]])
#Rtn[測該類型的用例節點；查不到時為空列表]
""")]
	public partial IList<ITestNode> FindByTesteeType(Type TesteeType);

	[Doc($$"""
#Sum[按被測函數名取節點。]
#Descr[
函數名應以 `nameof` 賦值，
手寫字符串在改名後會靜默查不到。

與 `{{nameof(TestIndex.FindByTesteeType)}}` 一樣：
取不到返回空列表，
取到時返回索引內部的列表本身，請勿外部修改。
]
#Params([[被測函數名]])
#Rtn[測該函數的用例節點；查不到時為空列表]
""")]
	public partial IList<ITestNode> FindByTesteeFnName(str TesteeFnName);

	[Doc($$"""
#Sum[往一對多的字典裏追加節點，同一個節點不重複加。]
#Descr[
鍵不存在時先建空列表，
這樣調用方不必先判斷鍵在不在。

去重靠 `List.Contains`；
`{{nameof(ITestNode)}}` 未重寫相等比較，
因此比的是*引用*——
同一個節點被多條路徑帶進來時只會記一次。
]
#TParams([鍵的類型，不能為空])
#Params([[目標字典],
	[鍵],
	[要追加的節點]])
""")]
	private static partial void AddToDict<TKey>(
		IDictionary<TKey, IList<ITestNode>> Dict
		,TKey Key
		,ITestNode Node
	) where TKey:notnull;
}

[Doc($$"""
#Sum[測試管理器：組裝測試樹、維護索引、提供查詢。]

#Descr[
職責只有三件：
組裝測試樹、
維護索引、
提供查詢。
它不跑用例（執行在 `{{nameof(TreeTestExecutor)}}`），
也不篩選（篩選在 `{{nameof(TestFilter)}}`）。

*它不管依賴從哪來。*
測試類的依賴由測試項目自己的全局依賴來源提供——
慣例是各測試項目自己寫一個靜態 `Di` 類，
測試類在自己的欄位初始化時用 `GetRSvc<T>()` 取。
因此本庫不引用、也不需要知道任何依賴注入相關的類型：
框架對依賴注入*零概念*。

*管理器自身不是測試類*，
故不實現 `{{nameof(ITester)}}`；
上層要收編下層時用 `{{nameof(TestMgr.RegisterSubMgr)}}`，
不要試圖把管理器當測試類註冊。
兩個測試類型的接口混用會讓測試樹上多出一層無用的「管理器節點」。

典型用法：
```cs
MyProj1TestMgr.Inst.Init();
var Hits = MyProj1TestMgr.Inst.Filter()
	.TesteeType<ISvcUser>()
	.TesteeFn(nameof(ISvcUser.Login))
	.Select();
await Hits.RunEtPrint();
```
]
""")]
public interface ITestMgr{
	[Doc($$"""
#Sum[測試樹的根。]
#Descr[
默認實現裏根的唯一名固定為 `Tests`，
使失敗輸出的路徑可讀。
篩選的默認範圍就是本節點。
]
""")]
	ITestNode Root{get;}

	[Doc($$"""
#Sum[索引。]
#Descr[
`{{nameof(TestMgr.Init)}}` 之後才包含全部節點；
`{{nameof(TestMgr.Init)}}` 之前只有根。
查詢與篩選一律走本索引，不重掃全樹。
]
""")]
	TestIndex Index{get;}

	[Doc($$"""
#Sum[收編另一個測試管理器的整棵子樹。]
#Descr[
只登記、不立刻掛樹：
真正的掛載與索引合併發生在本管理器的 `{{nameof(TestMgr.Init)}}` 裏。
這樣安排是因為本管理器的 `{{nameof(TestMgr.Init)}}` 會重建索引，
立刻併進來的條目會被那次重建清掉。

必須在本管理器 `{{nameof(TestMgr.Init)}}` *之前*調用；
`{{nameof(TestMgr.Init)}}` 之後再調用會直接拋例外。

下級管理器自身可以尚未 `{{nameof(TestMgr.Init)}}`，
`{{nameof(TestMgr.Init)}}` 時會代它初始化。
]
#Params([[要收編的下級管理器，不能為空；通常是各測試項目導出的單例]])
""")]
	void RegisterSubMgr(ITestMgr SubMgr);

	[Doc($$"""
#Sum[把已聲明的測試類實例化、掛到樹上，並建立索引；返回根節點。]
#Descr[
*只允許調用一次*，
第二次調用直接拋例外。

調用時機有硬性要求：
測試項目的全局依賴來源必須*先就緒*，
因為 `{{nameof(TestMgr.Init)}}` 會立刻 `new` 出測試類，
而測試類在欄位初始化時就取依賴。
順序顛倒會拿到空的依賴。

具體步驟見默認實現裏的註釋。
]
#Rtn[測試樹的根]
""")]
	ITestNode Init();
}

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
	[測試類節點的節點名；為空時由 `{{nameof(ExtnITestNode.NewTesterChild)}}` 取測試類短名],
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

[Doc($$"""
#Sum[測試樹的遍歷、查找與篩選入口的擴展方法。]
#Descr[
查找這幾個方法在取不到時*拋 `KeyNotFoundException`*，
與 `{{nameof(TestIndex)}}` 上那些「取不到返回空」的方法區分開：
擴展方法面向「這個東西必須存在」的場合，
取不到即為測試腳本寫錯。

遍歷這兩個方法是惰性的（`yield return`），
每次調用都重新走一遍樹，
不要多次消費同一個返回值。
]
""")]
public static partial class ExtnTestMgr{
	[Doc($$"""
#Sum[先序遍歷本節點及其全部遞迴子節點。]
#Descr[
含本節點自身。
次序即聲明次序，
篩選結果的穩定排序就靠這個次序。

*不防環*：
如果手工把節點接成環，本方法會無限遞迴；
按正常用法（`{{nameof(ExtnITestNode.NewChild)}}`、`{{nameof(ExtnITestNode.NewTesterChild)}}`）接出來的樹不會成環。
]
#Params([[遍歷起點，即 `this`]])
#Rtn[先序序列，含起點自身]
""")]
	public static partial IEnumerable<ITestNode> Walk(this ITestNode z);

	[Doc($$"""
#Sum[先序遍歷子樹，只取帶用例的節點。]
#Descr[
「帶用例」的判據是 `{{nameof(ITestNode.Data)}}` 非空；
分組節點與脫離原樹的臨時根都會被濾掉。
]
#Params([[遍歷起點，即 `this`]])
#Rtn[帶用例的節點序列，按先序]
""")]
	public static partial IEnumerable<ITestNode> WalkCases(this ITestNode z);

	[Doc($$"""
#Sum[按測試類找節點。]
#Descr[
「找不到就拋例外」的版本，
與 `{{nameof(TestIndex.FindByTesterType)}}` 的「找不到返回空」相對。
]
#Params([[測試管理器，即 `this`],
	[測試類類型]])
#Rtn[該測試類的節點]
""")]
	public static partial ITestNode GetNodeByTesterType(this ITestMgr z, Type TesterType);

	[Doc($$"""
#Sum[按唯一名找節點。]
#Descr[
節點名要可讀、且樹內唯一，才能真正按名定位；
沒顯式給名字的節點名是 ULID，基本查不到。
]
#Params([[測試管理器，即 `this`],
	[節點唯一名]])
#Rtn[該名字的節點]
""")]
	public static partial ITestNode GetNodeByName(this ITestMgr z, str UniqName);

	[Doc($$"""
#Sum[按被測類型找節點。]
#Descr[
一個被測類型可能對應多個節點（用例散在多個測試類下），
因此返回列表；
一條都沒有時拋例外。
]
#Params([[測試管理器，即 `this`],
	[被測類型]])
#Rtn[測該類型的用例節點]
""")]
	public static partial IList<ITestNode> GetNodesByTesteeType(this ITestMgr z, Type TesteeType);

	[Doc($$"""
#Sum[按被測函數名找節點。]
#Descr[
函數名要以 `nameof` 賦值，
手寫字符串改名後會靜默查不到，
在這裏表現為拋例外。
]
#Params([[測試管理器，即 `this`],
	[被測函數名]])
#Rtn[測該函數的用例節點]
""")]
	public static partial IList<ITestNode> GetNodesByTesteeFnName(this ITestMgr z, str TesteeFnName);

	[Doc($$"""
#Sum[開始一次篩選，範圍是整棵樹。]
#Descr[
篩選結果可直接執行：
```cs
await Mgr.Filter().TesteeFn(nameof(ISvcUser.Login)).Select().RunEtPrint();
```
]
#Params([[測試管理器，即 `this`]])
#Rtn[範圍為整棵樹的篩選器]
""")]
	public static partial TestFilter Filter(this ITestMgr z);

	[Doc($$"""
#Sum[在指定範圍內開始一次篩選。]
#Descr[
範圍之外的用例即使命中條件也不會被選中。
]
#Params([[測試管理器，即 `this`],
	[範圍子樹的根；傳根節點等於不限範圍]])
#Rtn[範圍為給定子樹的篩選器]
""")]
	public static partial TestFilter Filter(this ITestMgr z, ITestNode Scope);
}
