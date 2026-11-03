using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public partial class TestIndex{
	public partial void Add(ITestNode Node){
		Add(Node, Node is ITesterNode TesterNd ? TesterNd.TesterType : null);
	}

	public partial void Add(ITestNode Node, Type? TesterType){
		// step 1：唯一名這組。允許覆蓋同一個實例，撞名則直接失敗。
		if(Node.UniqName is {Length: > 0} Name){
			if(UniqName_TestNode.TryGetValue(Name, out var Old) && !ReferenceEquals(Old, Node)){
				throw new InvalidOperationException(
					$"重複的節點唯一名：{Name}"
				);
			}
			UniqName_TestNode[Name] = Node;
		}

		// step 2：測試類這組。一個測試類只能對應一個節點。
		if(TesterType is { } TT){
			if(TesterType_TestNode.TryGetValue(TT, out var Old) && !ReferenceEquals(Old, Node)){
				throw new InvalidOperationException(
					$"重複的測試類：{TT.FullName}"
				);
			}
			TesterType_TestNode[TT] = Node;
		}

		// step 3：用例元數據這兩組。
		// 元數據只掛在帶用例的葉節點上；
		// 分組節點的 Data 為空，
		// 因而不會進被測類型與被測函數名的索引。
		var Data = Node.Data;
		if(Data is null){
			return;
		}
		foreach(var TesteeType in Data.TesteeTypes){
			AddToDict(TesteeType_TestNode, TesteeType, Node);
		}
		foreach(var FnName in Data.TesteeFnNames){
			// 空白的函數名無意義，跳過以免污染索引、
			// 使之後按名字查到一堆不相關的節點。
			if(string.IsNullOrWhiteSpace(FnName)){
				continue;
			}
			AddToDict(TesteeFnName_TestNode, FnName, Node);
		}
	}

	public partial void MergeFrom(TestIndex Other, str? SkipUniqName){
		ArgumentNullException.ThrowIfNull(Other);
		// step 1：唯一名這組，逐條覆蓋；SkipUniqName 那條直接不放進來。
		foreach(var Pair in Other.UniqName_TestNode){
			if(Pair.Key == SkipUniqName){
				continue;
			}
			UniqName_TestNode[Pair.Key] = Pair.Value;
		}
		// step 2：測試類這組，一對一，直接覆蓋。
		foreach(var Pair in Other.TesterType_TestNode){
			TesterType_TestNode[Pair.Key] = Pair.Value;
		}
		// step 3：兩個一對多的組，按追加合併，已有的條目不動。
		foreach(var Pair in Other.TesteeType_TestNode){
			foreach(var Node in Pair.Value){
				AddToDict(TesteeType_TestNode, Pair.Key, Node);
			}
		}
		foreach(var Pair in Other.TesteeFnName_TestNode){
			foreach(var Node in Pair.Value){
				AddToDict(TesteeFnName_TestNode, Pair.Key, Node);
			}
		}
	}

	public partial ITestNode? FindByName(str UniqName){
		return UniqName_TestNode.TryGetValue(UniqName, out var Node) ? Node : null;
	}

	public partial ITestNode? FindByTesterType(Type TesterType){
		return TesterType_TestNode.TryGetValue(TesterType, out var Node) ? Node : null;
	}

	public partial IList<ITestNode> FindByTesteeType(Type TesteeType){
		return TesteeType_TestNode.TryGetValue(TesteeType, out var Nodes) ? Nodes : [];
	}

	public partial IList<ITestNode> FindByTesteeFnName(str TesteeFnName){
		return TesteeFnName_TestNode.TryGetValue(TesteeFnName, out var Nodes) ? Nodes : [];
	}

	private static partial void AddToDict<TKey>(
		IDictionary<TKey, IList<ITestNode>> Dict
		,TKey Key
		,ITestNode Node
	) where TKey:notnull{
		if(!Dict.TryGetValue(Key, out var List)){
			List = [];
			Dict[Key] = List;
		}
		if(!List.Contains(Node)){
			List.Add(Node);
		}
	}
}

public abstract partial class TestMgr{
	public partial void RegisterTester<T>(str? NodeName) where T:ITester,new(){
		AddPendingTester(typeof(T), () => new T(), NodeName, null);
	}

	public partial void RegisterTesterIntoBranch<T>(
		str BranchName, str? NodeName
	)
		where T:ITester,new()
	{
		ArgumentNullException.ThrowIfNull(BranchName);
		AddPendingTester(typeof(T), () => new T(), NodeName, BranchName);
	}

	private partial void AddPendingTester(
		Type TesterType
		,Func<ITester> MkTester
		,str? NodeName
		,str? BranchName
	){
		ArgumentNullException.ThrowIfNull(TesterType);
		ArgumentNullException.ThrowIfNull(MkTester);
		lock(_sync){
			EnsureNotInitialized();
			_pendingTesters.Add(new PendingTester(TesterType, MkTester, NodeName, BranchName));
		}
	}

	public partial ITestNode Init(){
		lock(_sync){
			// 前置校驗：初始化只允許一次。
			if(_initialized){
				throw new InvalidOperationException(
					"測試管理器已經初始化過，不應重複初始化。"
				);
			}

			// step 1：重建根與索引。
			// 索引整體重建，
			// 所以初始化之前就併進來的索引條目會統統消失
			// （`RegisterSubMgr` 因此只登記、到這裏才合併）。
			Root.UniqName = "Tests";
			Index = new TestIndex();
			Index.Add(Root);

			// step 2：先由子類聲明要註冊哪些測試類，再統一實例化。
			// 這樣子類的 SetupTree 裏可以任意安排聲明順序，
			// 而實例化的時機總在依賴來源就緒之後。
			//
			// 注意：_initialized 要在 SetupTree 返回之後才置真，
			// 否則子類在 SetupTree 裏調 RegisterTester 會
			// 被 EnsureNotInitialized 攔住。
			SetupTree();
			_initialized = true;

			// step 3：領域分組要在掛測試類之前先建好，
			// 否則測試類無處可掛。
			// 同名分組只建一次，故先去重。
			foreach(var BranchName in _pendingTesters
				.Select(x=>x.BranchName)
				.Where(x=>x is {Length: > 0})
				.Distinct()
			){
				AddBranch(BranchName!);
			}

			// step 4：逐個實例化測試類並掛樹；
			// 每個測試類掛完立刻把它的子樹入索引。
			foreach(var Pending in _pendingTesters){
				MountTester(Pending);
			}

			// step 5：測試類掛完之後才收編下級管理器，
			// 這樣合併出來的索引包含全部測試類的條目。
			foreach(var SubMgr in _pendingSubMgrs){
				MountSubMgr(SubMgr);
			}
			return Root;
		}
	}

	public partial ITestNode AddBranch(str BranchName){
		ArgumentNullException.ThrowIfNull(BranchName);
		// 已有同名節點就直接用，
		// 否則第二次建同名分組會撞唯一名而拋例外。
		if(Root.Children.FirstOrDefault(x=>x.UniqName == BranchName) is { } Existed){
			return Existed;
		}
		var R = Root.NewChild(BranchName);
		Index.Add(R);
		return R;
	}

	private partial void MountTester(PendingTester Pending){
		// step 1：定父節點。聲明了分組就掛到分組下，否則掛在根下。
		var Parent = Pending.BranchName is {Length: > 0} BranchName
			? AddBranch(BranchName)
			: Root;
		// step 2：實例化測試類並給它一個節點。
		var Tester = Pending.MkTester();
		var Node = Parent.NewTesterChild(Pending.TesterType, Pending.NodeName);
		Node.Tester = Tester;
		// step 3：讓測試類把用例掛上去。
		Tester.RegisterTestsInto(Node);
		// step 4：這一步新掛出來的東西才入索引。
		IndexSubtree(Node);
	}

	public partial void RegisterSubMgr(ITestMgr SubMgr){
		ArgumentNullException.ThrowIfNull(SubMgr);
		lock(_sync){
			EnsureNotInitialized();
			_pendingSubMgrs.Add(SubMgr);
		}
	}

	private partial void MountSubMgr(ITestMgr SubMgr){
		// step 1：先把子管理器初始化，它的樹與索引才是完整的。
		var SubRoot = SubMgr.Init();

			// step 2：新建一個承接節點。
			// 子管理器原本的根只是為了自己成樹而存在。
			// 真正掛上去時用一個以管理器類型名命名的新節點承接它的設定，
			// 避免兩棵樹共用同一個節點實例，
			// 也避免與父根撞名（父根的默認名也是 Tests）。
			var MountedRoot = Root.NewChild(SubMgr.GetType().Name);
			MountedRoot.Ordered = SubRoot.Ordered;
			MountedRoot.IsParallelRecursive = SubRoot.IsParallelRecursive;
			// step 3：把子根的子節點搬過來，並改寫它們的父引用。
			foreach(var Child in SubRoot.Children.ToList()){
				MountedRoot.Children.Add(Child);
				Child.Parents.Clear();
				Child.Parents.Add(MountedRoot);
			}

			// step 4：新子樹自己先入索引。
			Index.Add(MountedRoot);
			IndexSubtree(MountedRoot);
			// step 5：併入子管理器的索引。
			// 子管理器自己的索引裏有它根節點以下的全部條目，
			// 直接併進來，
			// 否則上層按測試類或按被測函數名查不到下級用例。
			// 子管理器的根節點條目要跳過：
			// 它已不在樹上，
			// 併進來會與父根同名互相覆蓋，
			// 而且不能靠「先併入再刪」補救——
			// 那會連父根自己的合法條目一起誤刪。
			Index.MergeFrom(SubMgr.Index, SkipUniqName: SubRoot.UniqName);
	}

	private partial void IndexSubtree(ITestNode SubRoot){
		foreach(var Node in SubRoot.Walk()){
			Index.Add(Node);
		}
	}

	private partial void EnsureNotInitialized(){
		if(_initialized){
			throw new InvalidOperationException(
				"測試管理器已經初始化過，不能再往裏註冊測試類。"
			);
		}
	}
}

public static partial class ExtnTestMgr{
	public static partial IEnumerable<ITestNode> Walk(this ITestNode z){
		yield return z;
		foreach(var Child in z.Children){
			foreach(var Descendant in Child.Walk()){
				yield return Descendant;
			}
		}
	}

	public static partial IEnumerable<ITestNode> WalkCases(this ITestNode z){
		foreach(var Node in z.Walk()){
			if(Node.Data is not null){
				yield return Node;
			}
		}
	}

	public static partial ITestNode GetNodeByTesterType(this ITestMgr z, Type TesterType){
		return z.Index.FindByTesterType(TesterType)
			?? throw new KeyNotFoundException($"找不到該測試類的節點：{TesterType.FullName}");
	}

	public static partial ITestNode GetNodeByName(this ITestMgr z, str UniqName){
		return z.Index.FindByName(UniqName)
			?? throw new KeyNotFoundException($"找不到該名字的節點：{UniqName}");
	}

	public static partial IList<ITestNode> GetNodesByTesteeType(this ITestMgr z, Type TesteeType){
		var R = z.Index.FindByTesteeType(TesteeType);
		if(R.Count < 1){
			throw new KeyNotFoundException($"找不到測該類型的用例：{TesteeType.FullName}");
		}
		return R;
	}

	public static partial IList<ITestNode> GetNodesByTesteeFnName(this ITestMgr z, str TesteeFnName){
		var R = z.Index.FindByTesteeFnName(TesteeFnName);
		if(R.Count < 1){
			throw new KeyNotFoundException($"找不到測該函數的用例：{TesteeFnName}");
		}
		return R;
	}

	public static partial TestFilter Filter(this ITestMgr z){
		return new TestFilter(z);
	}

	public static partial TestFilter Filter(this ITestMgr z, ITestNode Scope){
		return new TestFilter(z, Scope);
	}
}
