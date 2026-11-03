using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public partial class TestFilter{
	public partial TestFilter(ITestMgr Mgr, ITestNode? Scope){
		ArgumentNullException.ThrowIfNull(Mgr);
		this.Mgr = Mgr;
		this.Scope = Scope ?? Mgr.Root;
	}

	public partial TestFilter Under(ITestNode Scope){
		ArgumentNullException.ThrowIfNull(Scope);

		// step 1：造一個帶同樣條件的副本，原篩選器不動。
		var R = new TestFilter(Mgr, Scope){
			_TesterType = _TesterType,
			_TesteeType = _TesteeType,
			_TesteeFnName = _TesteeFnName,
			_NameContains = _NameContains,
		};
		// step 2：自訂謂詞是委託，逐個搬過去；
		// 列表是 readonly 的，只能加、不能整體賦值。
		foreach(var Cond in _ExtraConditions){
			R._ExtraConditions.Add(Cond);
		}
		return R;
	}

	public partial TestFilter TesterType(Type TesterType){
		_TesterType = TesterType;
		return this;
	}

	public partial TestFilter TesterType<TTester>(){
		return TesterType(typeof(TTester));
	}

	public partial TestFilter TesteeType(Type TesteeType){
		_TesteeType = TesteeType;
		return this;
	}

	public partial TestFilter TesteeType<TTestee>(){
		return TesteeType(typeof(TTestee));
	}

	public partial TestFilter TesteeFn(str TesteeFnName){
		_TesteeFnName = TesteeFnName;
		return this;
	}

	public partial TestFilter NameContains(str Fragment){
		_NameContains = Fragment;
		return this;
	}

	public partial TestFilter Where(Func<ITestCase, bool> Predicate){
		ArgumentNullException.ThrowIfNull(Predicate);
		_ExtraConditions.Add(Predicate);
		return this;
	}

	public partial IList<ITestNode> Apply(){
		// step 1：按索引取候選；此時已受 Scope 限制。
		var Candidates = PickCandidates();

		// step 2：逐條套用全部條件。
		// 候選可能來自多個索引組，同一個節點會被帶進來多次，
		// 因此用*引用*相等去重（ITestNode 未重寫相等比較）。
		var Seen = new HashSet<ITestNode>(ReferenceEqualityComparer.Instance);
		var R = new List<ITestNode>();
		foreach(var Node in Candidates){
			// 分組節點、臨時根沒有用例，不是篩選對象。
			if(Node.Data is not ITestCase Case){
				continue;
			}
			if(!Seen.Add(Node)){
				continue;
			}
			if(!Match(Case)){
				continue;
			}
			R.Add(Node);
		}

		// step 3：按樹上的先序位置排序，
		// 使同一次篩選的結果次序穩定，
		// 逐次比對時不會因索引順序不同而抖動。
		//
		// 先給 Scope 内每個節點編個序號，
		// 排序時直接查表，
		// 免得兩兩比較時反覆走樹。
		var Order = new Dictionary<ITestNode, i32>(ReferenceEqualityComparer.Instance);
		var Seq = 0;
		foreach(var Node in Scope.Walk()){
			Order[Node] = Seq++;
		}
		// 查不到序號的節點（正常不該出現）一律排到最後，
		// 至少保證排序結果仍然是確定的。
		R.Sort((A, B)=>{
			var OA = Order.TryGetValue(A, out var X) ? X : i32.MaxValue;
			var OB = Order.TryGetValue(B, out var Y) ? Y : i32.MaxValue;
			return OA.CompareTo(OB);
		});
		return R;
	}

	public partial TestSelection Select(){
		return new TestSelection(Mgr, Apply());
	}

	public partial void PrintHits(){
		var Hits = Apply();
		Console.WriteLine($"命中共 {Hits.Count} 條用例：");
		foreach(var Node in Hits){
			Console.WriteLine($"  {Node.PathString()}");
		}
	}

	private partial IList<ITestNode> PickCandidates(){
		// step 1：有測試類條件時，以它為最強的起點。
		// 索引裏直接就有該測試類的子樹，
		// 掃的量最小。
		if(_TesterType is { } TT){
			var TesterNode = Mgr.Index.FindByTesterType(TT);
			if(TesterNode is null){
				return [];
			}
			IList<ITestNode> Candidates;
			if(_TesteeFnName is { } FnName){
				// 測試類的子樹 ∩ 測過該函數的節點。
				Candidates = Intersect(
					TesterNode.WalkCases().ToList()
					,Mgr.Index.FindByTesteeFnName(FnName)
				);
			}
			else if(_TesteeType is { } Testee){
				// 測試類的子樹 ∩ 測過該類型的節點。
				Candidates = Intersect(
					TesterNode.WalkCases().ToList()
					,Mgr.Index.FindByTesteeType(Testee)
				);
			}
			else{
				// 只有測試類條件，整個子樹的用例都是候選。
				Candidates = TesterNode.WalkCases().ToList();
			}
			// 與其它條件組合時仍要遵守 Under 的範圍限制。
			return Restrict(Candidates);
		}

		// step 2：沒有測試類條件，但能用被測函數名或被測類型。
		if(_TesteeFnName is { } Fn){
			var ByFn = Mgr.Index.FindByTesteeFnName(Fn);
			if(_TesteeType is { } Testee2){
				return Restrict(Intersect(ByFn, Mgr.Index.FindByTesteeType(Testee2)));
			}
			return Restrict(ByFn);
		}

		if(_TesteeType is { } Testee3){
			return Restrict(Mgr.Index.FindByTesteeType(Testee3));
		}

		// step 3：一個索引條件都沒有，只能走範圍内的整棵子樹。
		return Scope.WalkCases().ToList();
	}

	private partial IList<ITestNode> Restrict(IList<ITestNode> Nodes){
		// 範圍是整棵樹時，索引挑出來的東西必然在範圍内。
		if(ReferenceEquals(Scope, Mgr.Root)){
			return Nodes;
		}
		var InScope = new HashSet<ITestNode>(Scope.Walk(), ReferenceEqualityComparer.Instance);
		return Nodes.Where(x=>InScope.Contains(x)).ToList();
	}

	private static partial IList<ITestNode> Intersect(IList<ITestNode> A, IList<ITestNode> B){
		var BSet = new HashSet<ITestNode>(B, ReferenceEqualityComparer.Instance);
		return A.Where(x=>BSet.Contains(x)).ToList();
	}

	private partial bool Match(ITestCase Case){
		// 測試類：相等，不是「派生」。
		if(_TesterType is { } TT && Case.TesterType != TT){
			return false;
		}
		// 被測類型：用例元數據裏聲明過的類型之中包含它即可。
		if(_TesteeType is { } Testee && !Case.TesteeTypes.Contains(Testee)){
			return false;
		}
		// 被測函數名：同上。
		if(_TesteeFnName is { } Fn && !Case.TesteeFnNames.Contains(Fn)){
			return false;
		}
		// 名字片段：不分大小寫；空片段視為沒設條件。
		if(_NameContains is {Length: > 0} Fragment
			&& !Case.UniqName.Contains(Fragment, StringComparison.OrdinalIgnoreCase)
		){
			return false;
		}
		// 自訂謂詞：全部都要通過。
		foreach(var Cond in _ExtraConditions){
			if(!Cond(Case)){
				return false;
			}
		}
		return true;
	}
}
