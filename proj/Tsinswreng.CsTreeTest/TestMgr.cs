using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：TestMgr 的成員宣告；實現見 TestMgr.Impl.cs。
[Doc($$"""
測試管理器的默認實現：子類只需重寫 `SetupTree` 聲明要註冊哪些測試類。

```cs
public class MyProj1TestMgr : TestMgr{
	public static MyProj1TestMgr Inst = new();
	protected override void SetupTree(){
		RegisterTester<TestCalculator>();                 // 掛在根下
		RegisterTesterIntoBranch<TestSvcUser>("SvcUser"); // 歸到分組下
		RegisterSubMgr(OtherTestMgr.Inst);                // 收編下級管理器
	}
}
```
生命週期分兩段：`SetupTree` 只登記，`{{nameof(Init)}}` 才實例化、掛樹、建索引。
入口順序：先備好全局依賴來源 → `{{nameof(Init)}}()` → 交給執行器跑。
""")]
public abstract partial class TestMgr : ITestMgr{
	[Doc($$"""
一個待註冊的測試類：類型 + 「怎麼 new」。用委託是因為 AOT 下不能反射實例化。
""")]
	private sealed record PendingTester(
		Type TesterType
		,Func<ITester> MkTester
		,str? NodeName
		,str? BranchName
	);

	[Doc($$"""
待註冊的測試類隊列；`{{nameof(Init)}}` 時才實例化。
""")]
	private readonly List<PendingTester> _pendingTesters = [];

	[Doc($$"""
待收編的下級管理器隊列；`{{nameof(Init)}}` 時才掛樹。
""")]
	private readonly List<ITestMgr> _pendingSubMgrs = [];

	[Doc($$"""
保護三個待初始化字段的鎖，避免一邊登記一邊初始化。
""")]
	private readonly object _sync = new();

	[Doc($$"""
是否已初始化。置真後禁止再登記；必須在 `SetupTree()` 返回之後才置真。
""")]
	private bool _initialized = false;

	[Doc($$"""
測試樹的根，默認名 `Tests`。要換根得在 `{{nameof(Init)}}` 之前。
""")]
	public ITestNode Root{get;protected set;} = new TestNode{UniqName = "Tests"};

	[Doc($$"""
索引；`{{nameof(Init)}}` 時整體重建。
""")]
	public TestIndex Index{get;protected set;} = new();

	[Doc($$"""
子類在此聲明要註冊的測試類、領域分組與下級管理器。

由 `{{nameof(Init)}}` 調用一次。只做聲明：別在這裏跑用例，也別直接往 `{{nameof(Root)}}` 掛節點。
""")]
	protected abstract void SetupTree();

	[Doc($$"""
註冊一個測試類，掛在根下。

測試類由管理器 `new`（無參構造，依賴自取），因此不得有帶參構造子。
只登記，`{{nameof(Init)}}` 時才實例化；`{{nameof(Init)}}` 之後調用拋例外。

#TPrm[{{nameof(T)}}][測試類類型，須實作 `{{nameof(ITester)}}` 且有無參構造子]
#Prm[{{nameof(NodeName)}}][節點名，為空時取測試類短名]
""")]
	public partial void RegisterTester<T>(str? NodeName = null) where T:ITester,new();

	[Doc($$"""
註冊一個測試類，歸到領域分組下；同名分組共用一個節點。

#TPrm[{{nameof(T)}}][測試類類型，須實作 `{{nameof(ITester)}}` 且有無參構造子]
#Prm[{{nameof(BranchName)}}][分組名，不能為空]
#Prm[{{nameof(NodeName)}}][節點名，為空時取測試類短名]
""")]
	public partial void RegisterTesterIntoBranch<T>(
		str BranchName, str? NodeName = null
	)
		where T:ITester,new();

	[Doc($$"""
待註冊測試類的唯一入口；「初始化後禁止登記」只在這裏把一次。
""")]
	private partial void AddPendingTester(
		Type TesterType
		,Func<ITester> MkTester
		,str? NodeName
		,str? BranchName
	);

	[Doc($$"""
初始化：重建索引、實例化測試類並掛樹、收編下級管理器。只允許一次。

`_initialized` 要在 `SetupTree()` 返回之後才置真；下級管理器要在測試類全部掛完之後才收編——否則會靜默丟東西。

#Rtn[測試樹的根]
""")]
	public partial ITestNode Init();

	[Doc($$"""
在根下取或建領域分組節點；已存在同名分組時直接返回它。

#Prm[{{nameof(BranchName)}}][分組名，同時作為節點唯一名]
#Rtn[分組節點]
""")]
	public partial ITestNode AddBranch(str BranchName);

	[Doc($$"""
實例化測試類、掛節點、讓它註冊用例，最後把新子樹入索引。
""")]
	private partial void MountTester(PendingTester Pending);

	[Doc($$"""
登記一個待收編的下級管理器；只登記，`{{nameof(Init)}}` 時才掛樹，且必須在 `{{nameof(Init)}}` 之前調用。
""")]
	public partial void RegisterSubMgr(ITestMgr SubMgr);

	[Doc($$"""
把已登記的下級管理器整棵掛到本樹上。用子管理器類型名的新節點承接，避免與父根同名或兩樹共用節點。同名子管理器只能收編一次。
""")]
	private partial void MountSubMgr(ITestMgr SubMgr);

	[Doc($$"""
以先序把子樹的每個節點入索引。
""")]
	private partial void IndexSubtree(ITestNode SubRoot);

	[Doc($$"""
初始化之後拒絕再登記，直接拋例外。
""")]
	private partial void EnsureNotInitialized();
}
