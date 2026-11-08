using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Drafts;

/// <summary>
/// 候選管理器：與庫本體的 <see cref="TestMgr"/> 差別只有一件事——
/// *沒有「初始化」這一步*。
/// </summary>
/// <remarks>
/// 為甚麼能去掉：
/// 庫本體的兩段式（先 <c>SetupTree</c> 聲明、再 <c>Init()</c> 實例化）只有一個理由——
/// 測試類在*欄位初始化*時就取依賴，所以必須等依賴來源就緒才敢 new 它。
/// 若依賴改成*用例執行時*才取，就沒有任何東西需要在建樹前就緒，
/// 樹可以在第一次用到的時候直接建好。
///
/// 建樹時機：第一次讀 <see cref="Root"/> 。
/// 用懶建而不是建構子裏建，是為了避開「基底建構子裏虛調 SetupTree」這個坑
/// （派生類的欄位初始化器還沒跑）。
/// </remarks>
public abstract class TestMgrV2{
	private readonly object _Sync = new();
	private readonly List<TestMgrV2> _SubMgrs = [];
	private ITestNode? _Root;

	/// <summary>測試樹的根。第一次讀它就建樹；之後讀到的是同一棵樹。</summary>
	public ITestNode Root{
		get{
			EnsureBuilt();
			return _Root!;
		}
	}

	/// <summary>索引。與 <see cref="Root"/> 同時建立，建好之後包含整棵樹的節點。</summary>
	/// <summary>
	/// 兼容 <see cref="TestMgrV2"/> 的形狀。本版不需要「初始化」這一步，
	/// 調它只是取根（已經建好就原樣返回，可重複調用）。
	/// </summary>
	public ITestNode Init() => Root;

	/// <summary>
	/// 子類在此聲明樹長甚麼樣：掛哪些測試類、開哪些領域分組、收編哪些下級管理器。
	/// </summary>
	/// <remarks>
	/// 本方法在「第一次讀 <see cref="Root"/>」時被調用一次。
	/// 可以放心用 <see cref="AddBranch"/> 與各寫法自己的 <c>RegisterTester...</c>，
	/// 它們都掛在正在建的這棵樹上。
	/// </remarks>
	protected abstract void SetupTree();

	/// <summary>收編另一個測試管理器。必須在 <see cref="SetupTree"/> 裏聲明。</summary>
	/// <param name="SubMgr">下級管理器。</param>
	public void RegisterSubMgr(TestMgrV2 SubMgr){
		ArgumentNullException.ThrowIfNull(SubMgr);
		lock(_Sync){
			if(_Root is not null){
				throw new InvalidOperationException(
					"樹已建好，不能再收編下級管理器。請在 SetupTree 裏聲明。"
				);
			}
			_SubMgrs.Add(SubMgr);
		}
	}

	/// <summary>在根下取／建一個領域分組。同名返回同一個節點。</summary>
	/// <param name="BranchName">分組名，同時作為節點唯一名。</param>
	public ITestNode AddBranch(str BranchName){
		ArgumentNullException.ThrowIfNull(BranchName);
		var Root = this.Root;
		if(Root.Children.FirstOrDefault(x => x.UniqName == BranchName) is { } Existed){
			return Existed;
		}
		return Root.NewChild(BranchName);
	}

	/// <summary>
	/// 給測試類建一個節點：指定了分組就掛到分組下，否則掛在根下。
	/// </summary>
	/// <param name="TesterType">測試類類型，寫進節點與索引。</param>
	/// <param name="NodeName">節點名；為空時取測試類短名。</param>
	/// <param name="BranchName">領域分組名；為空表示掛在根下。</param>
	protected ITesterNode MkTesterNode(Type TesterType, str? NodeName, str? BranchName){
		ArgumentNullException.ThrowIfNull(TesterType);
		var Parent = BranchName is { Length: > 0 } ? AddBranch(BranchName) : this.Root;
		return Parent.NewTesterChild(TesterType, NodeName);
	}

	private void EnsureBuilt(){
		lock(_Sync){
			if(_Root is not null){
				return;
			}
			// 先記下根再調 SetupTree：
			// 這樣 SetupTree 裏讀 Root 不會遞迴、也不會拿到空值。
			var Root = new TestNode{ UniqName = "Tests" };
			_Root = Root;

			SetupTree();

			foreach(var SubMgr in _SubMgrs){
				MountSubMgr(Root, SubMgr);
			}

		}
	}

	/// <summary>
	/// 把下級管理器整棵樹掛過來。
	/// </summary>
	/// <remarks>
	/// 用一個以子管理器類型名命名的新節點承接，
	/// 因為子管理器自己的根只為它自己成樹而存在，
	/// 直接接上來會撞名（父根也叫 <c>Tests</c>）。
	/// </remarks>
	private static void MountSubMgr(ITestNode Root, TestMgrV2 SubMgr){
		var SubRoot = SubMgr.Init();
		var Mounted = Root.NewChild(SubMgr.GetType().Name);
		Mounted.Ordered = SubRoot.Ordered;
		Mounted.IsParallelRecursive = SubRoot.IsParallelRecursive;
		foreach(var Child in SubRoot.Children.ToList()){
			Mounted.Children.Add(Child);
			Child.Parents.Clear();
			Child.Parents.Add(Mounted);
		}
	}
}
