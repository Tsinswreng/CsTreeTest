using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

[Doc("""
#Sum[按條件把要跑的那批用例挑出來。]

#Descr[
用途：
不必記住哪條用例在哪個節點下，
直接按條件把要跑的那批用例挑出來。

*篩選走測試管理器的索引*，
不重掃全樹；
只有「限定範圍」與「最後排序」這兩步要遍歷 `Scope` 的子樹。

*多個條件之間是「同時滿足」（AND）的關係。*
需要更複雜的組合時，
用 `Where` 傳一個自訂謂詞進來。

*與 `Under(範圍)` 組合時仍然受範圍限制。*
先按索引把範圍縮到候選，
再把候選限制回 `Scope` 之内，
所以範圍之外的用例即使命中條件也不會被選中。

*結果穩定排序*：
按樹上的先序次序排列，
逐次比對時不會因索引順序不同而抖動。

典型用法：
```cs
var Hits = Mgr.Filter()
	.TesteeType<ISvcUser>()
	.TesteeFn(nameof(ISvcUser.Login))
	.Select();

Hits.PrintHits();       // 先看一眼再決定跑不跑
await Hits.RunEtPrint();// 確認無誤再跑
```
]
""")]
public class TestFilter{
	[Doc("""
#Sum[篩選所依據的測試管理器。]
#Descr[
索引與候選集都從它取。
構造時即固定，之後不可換。
]
""")]
	private readonly ITestMgr Mgr;

	[Doc("""
#Sum[起始的候選節點，即篩選範圍。]
#Descr[
預設是整棵樹的根，
用 `Under` 可以把範圍收窄到某一棵子樹。

範圍是*硬邊界*：
索引挑出來的候選若不在 `Scope` 的子樹内，一律被剔掉。
]
""")]
	private readonly ITestNode Scope;

	[Doc("""
#Sum[要求用例屬於這個測試類。]
#Descr[
為空表示不限制。
同時作為候選集的來源（索引裏以它為起點）。
]
""")]
	private Type? _TesterType;

	[Doc("""
#Sum[要求用例測過這個被測類型。]
#Descr[
為空表示不限制。
]
""")]
	private Type? _TesteeType;

	[Doc("""
#Sum[要求用例測過這個被測函數名。]
#Descr[
為空表示不限制。
應以 `nameof` 賦值，手寫字符串改名後會靜默失效。
]
""")]
	private str? _TesteeFnName;

	[Doc("""
#Sum[要求用例名包含這段文字。]
#Descr[
比對不分大小寫（`OrdinalIgnoreCase`）。
用不上索引，
因此只在逐條匹配時判斷。
]
""")]
	private str? _NameContains;

	[Doc("""
#Sum[額外的自訂條件。]
#Descr[
每個謂詞都與其它條件*同時滿足*（AND）：
任何一個返回 `false` 的用例都會被剔掉。
要表達「滿足其一」時，
把整個或關係寫進*一個*謂詞裏。

謂詞只拿到用例元數據（`ITestCase`），
拿不到節點本身，
因此無法按節點路徑或樹上的位置判斷。
]
""")]
	private readonly List<Func<ITestCase, bool>> _ExtraConditions = [];

	[Doc("""
#Sum[造一個篩選器。]
#Descr[
一般不直接 new，
用 `ITestMgr` 上的 `Filter` 擴展方法造。
]
#Params([[篩選所依據的測試管理器，不能為空],
	[篩選範圍的根；為空時取整個測試樹的根]])
""")]
	public TestFilter(ITestMgr Mgr, ITestNode? Scope = null){
		ArgumentNullException.ThrowIfNull(Mgr);
		this.Mgr = Mgr;
		this.Scope = Scope ?? Mgr.Root;
	}

	[Doc("""
#Sum[把篩選範圍收窄到某一棵子樹，返回一個*新的*篩選器。]
#Descr[
例如先按領域取到那個分組節點，
再在這個分組裏篩。

*本方法不改變原篩選器*：
已設好的條件被複製進新對象，
原對象可以繼續用；
之後再加的條件只影響新對象。

條件是複製而不是共享，
所以兩個篩選器可以各自繼續加條件而不互相影響。
]
#Params([[子樹的根，不能為空；傳測試樹的根等於不限範圍]])
#Rtn[範圍為給定子樹的新篩選器；已設好的條件原樣帶過去]
""")]
	public TestFilter Under(ITestNode Scope){
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

	[Doc("""
#Sum[限定測試類（與其它條件同時滿足）。]
#Descr[
在同一個篩選器上重複調用會覆蓋前一次的值。

傳 `null` 等於取消本條件（本方法不做非空校驗）；
索引裏查不到該測試類時結果直接為空，不拋例外。
]
#Params([[測試類類型]])
#Rtn[本篩選器，便於鍊式調用]
""")]
	public TestFilter TesterType(Type TesterType){
		_TesterType = TesterType;
		return this;
	}

	[Doc("""
#Sum[限定測試類（與其它條件同時滿足），類型由泛型參數給。]
#Descr[
避免手寫 `typeof`，
也能讓編譯期檢查類型名。
]
#TParams([測試類類型])
#Rtn[本篩選器，便於鍊式調用]
""")]
	public TestFilter TesterType<TTester>(){
		return TesterType(typeof(TTester));
	}

	[Doc("""
#Sum[限定被測類型（與其它條件同時滿足）。]
#Descr[
在同一個篩選器上重複調用會覆蓋前一次的值。

傳 `null` 等於取消本條件。
]
#Params([[被測類型]])
#Rtn[本篩選器，便於鍊式調用]
""")]
	public TestFilter TesteeType(Type TesteeType){
		_TesteeType = TesteeType;
		return this;
	}

	[Doc("""
#Sum[限定被測類型（與其它條件同時滿足），類型由泛型參數給。]
#Descr[
最常用的寫法：
```cs
Mgr.Filter().TesteeType<ISvcUser>().Select();
```
]
#TParams([被測類型])
#Rtn[本篩選器，便於鍊式調用]
""")]
	public TestFilter TesteeType<TTestee>(){
		return TesteeType(typeof(TTestee));
	}

	[Doc("""
#Sum[限定被測函數名（與其它條件同時滿足）。]
#Descr[
*應以 `nameof` 賦值*：
```cs
Mgr.Filter().TesteeFn(nameof(ISvcUser.Login)).Select();
```
手寫字符串在方法改名後*不會報錯*，
只會篩出零條用例。

在同一個篩選器上重複調用會覆蓋前一次的值。
]
#Params([[被測函數名]])
#Rtn[本篩選器，便於鍊式調用]
""")]
	public TestFilter TesteeFn(str TesteeFnName){
		_TesteeFnName = TesteeFnName;
		return this;
	}

	[Doc("""
#Sum[限定用例名包含某段文字（與其它條件同時滿足）。]
#Descr[
例如找名字裏帶 `SoftDel` 的全部用例。

比對不分大小寫；
比的是*完整用例名*（含前綴），
不是它的某一段。

在同一個篩選器上重複調用會覆蓋前一次的值。
]
#Params([[要包含的文字片段]])
#Rtn[本篩選器，便於鍊式調用]
""")]
	public TestFilter NameContains(str Fragment){
		_NameContains = Fragment;
		return this;
	}

	[Doc("""
#Sum[再加一個自訂條件（與其它條件同時滿足）。]
#Descr[
需要表達「滿足其一」或「排除某些」時用它。

*多個 `Where` 之間仍是 AND*：
要「或」就把或關係寫進一個謂詞裏。

謂詞拿到的只有用例元數據（`ITestCase`），
拿不到節點，因此判不了路徑。

例：
```cs
Mgr.Filter()
	.TesteeType<ISvcUser>()
	.Where(c => !c.UniqName.Contains("Slow"))
	.Select();
```
]
#Params([[謂詞；返回 `true` 表示這條用例通過本條件，不能為空]])
#Rtn[本篩選器，便於鍊式調用]
""")]
	public TestFilter Where(Func<ITestCase, bool> Predicate){
		ArgumentNullException.ThrowIfNull(Predicate);
		_ExtraConditions.Add(Predicate);
		return this;
	}

	[Doc("""
#Sum[取出命中的用例節點，按樹的先序次序排好。]
#Descr[
先按索引把範圍縮到候選，
再逐條套用全部條件（含 `Scope` 限制）。

只有 `Data` 是 `ITestCase` 的節點才算用例；
同一個節點被多條候選路徑帶進來時只算一次。

返回的列表是新建的，可以自由改動，
它與索引無關。
]
#Rtn[命中的用例節點，按樹的先序次序排列]
""")]
	public IList<ITestNode> Apply(){
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

	[Doc("""
#Sum[把命中的用例包成可直接執行的選集。]
#Descr[
比 `Apply()` 多的只是「能直接跑」這層封裝；
要自己遍歷節點時用 `Apply()`。
]
#Rtn[選集；可打印、可執行]
""")]
	public TestSelection Select(){
		return new TestSelection(Mgr, Apply());
	}

	[Doc("""
#Sum[只列印命中了哪些用例，不執行。]
#Descr[
用在不確定條件寫得對不對、
想先看一眼再決定跑不跑的場合。

輸出是每條用例的完整路徑，
判條件是否寫對比對條數更可靠。
]
""")]
	public void PrintHits(){
		var Hits = Apply();
		Console.WriteLine($"命中共 {Hits.Count} 條用例：");
		foreach(var Node in Hits){
			Console.WriteLine($"  {Node.PathString()}");
		}
	}

	[Doc("""
#Sum[先在索引裏挑候選：能用索引的條件就先用，以少掃一些節點。]
#Descr[
分支按「能用上索引的條件」區分：

+ 有測試類條件時：
	以該測試類的子樹為底（`WalkCases`），
	若還有被測函數名或被測類型，
	就與索引裏那一組求交集。
+ 沒有測試類、但有被測函數名或被測類型時：
	直接取索引裏的那一組；
	兩者都有就求交集。
+ 一個能用索引的條件都沒有時：
	退化成遍歷 `Scope` 的子樹。

*測試類那條路徑查不到節點時直接返回空*，
不必再往後走。

*無論走哪條分支，結果都經過 `Restrict`*，
保證與其它條件組合時仍受 `Under` 範圍限制。

`名包含` 與 `Where` 這兩類條件用不上索引，
一律留到逐條匹配時判斷。
]
#Rtn[候選節點；可能含不相關的節點，交由逐條匹配篩掉]
""")]
	private IList<ITestNode> PickCandidates(){
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

	[Doc("""
#Sum[把候選限制在 `Scope` 之内。]
#Descr[
範圍就是整棵樹時直接原樣返回，
不做多餘的遍歷。

收窄範圍時先把 `Scope` 子樹上的節點收進哈希集，
再逐個判斷候選在不在裏面；
比對用*引用*相等。
]
#Params([[索引挑出來的候選節點]])
#Rtn[落在 `Scope` 内（含 `Scope` 自身）的候選]
""")]
	private IList<ITestNode> Restrict(IList<ITestNode> Nodes){
		// 範圍是整棵樹時，索引挑出來的東西必然在範圍内。
		if(ReferenceEquals(Scope, Mgr.Root)){
			return Nodes;
		}
		var InScope = new HashSet<ITestNode>(Scope.Walk(), ReferenceEqualityComparer.Instance);
		return Nodes.Where(x=>InScope.Contains(x)).ToList();
	}

	[Doc("""
#Sum[求兩個集合的交集。]
#Descr[
保留第一個集合的次序，
這樣候選的次序仍然跟着「更強的那個條件」走；
反正最後 `Apply` 會統一再排一次。
]
#Params([[第一個集合，次序以它為準],
	[第二個集合]])
#Rtn[兩者共有的節點，按 `A` 的次序]
""")]
	private static IList<ITestNode> Intersect(IList<ITestNode> A, IList<ITestNode> B){
		var BSet = new HashSet<ITestNode>(B, ReferenceEqualityComparer.Instance);
		return A.Where(x=>BSet.Contains(x)).ToList();
	}

	[Doc("""
#Sum[判斷一條用例是否滿足全部條件（AND）。]
#Descr[
任一條件不滿足立即返回 `false`。
為空/未設的條件不參與判斷。

用例名比對不分大小寫（`OrdinalIgnoreCase`）；
其餘都是集合包含或類型相等。
]
#Params([[要判斷的用例元數據]])
#Rtn[全部條件是否都滿足]
""")]
	private bool Match(ITestCase Case){
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

[Doc("""
#Sum[一批已選中的用例。篩選與執行之間的交接物。]

#Descr[
職責邊界：
篩選只負責把用例挑出來（`TestFilter`），
執行只負責跑（`TreeTestExecutor`），
兩者靠本類型銜接。
所以可以「先 `Select()` 拿到選集，
看一眼或改一改，
再決定甚麼時候 `Run`」。

*`Nodes` 仍在原樹上*：
挑選不從原樹摘除節點，
以免影響後續再篩選。
執行時另造一棵脫離原樹的臨時樹（見 `MkDetachedTree`）。
]
""")]
public class TestSelection{
	[Doc("""
#Sum[選中的用例節點。]
#Descr[
節點仍留在原樹上，
不從原樹摘除，
以免影響後續再篩選。

列表由調用方持有，
增刪它會直接影響 `Count` 與 `Run` 的範圍。
]
""")]
	public IList<ITestNode> Nodes{get;}

	[Doc("""
#Sum[所屬的測試管理器。]
#Descr[
本類型自己不查索引，
需要索引的場合（例如調用方自行再篩）從這裏取。
]
""")]
	public ITestMgr Mgr{get;}

	[Doc("""
#Sum[造一個選集。]
#Descr[
一般不直接 new，
用 `TestFilter.Select()` 造。

本類型不複製 `Nodes`，
之後改那個列表等於改本選集。
]
#Params([[所屬測試管理器，不能為空],
	[選中的用例節點，不能為空；可以傳空列表表示一條都沒選中]])
""")]
	public TestSelection(ITestMgr Mgr, IList<ITestNode> Nodes){
		ArgumentNullException.ThrowIfNull(Mgr);
		ArgumentNullException.ThrowIfNull(Nodes);
		this.Mgr = Mgr;
		this.Nodes = Nodes;
	}

	[Doc(@"#Sum[命中的用例條數，即 `Nodes` 的條數。]")]
	public i32 Count => Nodes.Count;

	[Doc("""
#Sum[把命中的用例打印出來，不執行。]
#Descr[
用在不確定篩選條件寫得對不對、
想先看一眼再決定跑不跑的場合。

用在 `TestFilter.PrintHits` 之後：
那一步判斷條件，
這一步確認最終選集。
]
""")]
	public void PrintHits(){
		Console.WriteLine($"命中共 {Count} 條用例：");
		foreach(var Node in Nodes){
			Console.WriteLine($"  {Node.PathString()}");
		}
	}

	[Doc("""
#Sum[取命中的用例本身的元數據，供調用方自行查看。]
#Descr[
惰性枚舉（`yield return`），
一次調用只算一遍，
不要多次消費同一個返回值。

只取 `Data` 是 `ITestCase` 的節點。
]
#Rtn[用例元數據序列，次序與 `Nodes` 相同]
""")]
	public IEnumerable<ITestCase> Cases(){
		foreach(var Node in Nodes){
			if(Node.Data is ITestCase Case){
				yield return Case;
			}
		}
	}

	[Doc("""
#Sum[把選中的用例拼成一棵可直接執行、脫離原樹的臨時樹。]
#Descr[
*為甚麼要另造樹*：
挑選結果在樹上可能四散、拼不回單一根節點，
而執行器要的是節點。

*造出來的樹是扁平的*：
新建一個脫離原樹的根（唯一名 `SelectedCases`），
把命中的用例按 `Nodes` 的次序逐個掛成一級子節點。
代價是原樹上那些分組節點的
*順序與並行設定不再適用*，
這是已知取捨；
需要順序或並行時，
由調用方在 `OptTestExecutor` 上指定（見 `MakeOpt`）。

*用例會複製一份*：
`TestCase` 帶着可變的列表欄位，
複製一次可避免臨時樹與原樹共享同一個用例物件。
]
#Rtn[臨時樹的根；唯一名為 `SelectedCases`，其子節點即選中的用例]
""")]
	public ITestNode MkDetachedTree(){
		// step 1：造一個脫離原樹的根。
		var Root = new TestNode{UniqName = "SelectedCases"};

		// step 2：逐條用例掛成一級子節點，保持 `Nodes` 的次序。
		foreach(var Node in Nodes){
			if(Node.Data is not ITestCase Case){
				continue;
			}
			// 用例本身是純數據，
			// 但 TestCase 帶着可變的列表欄位，
			// 因此複製一份再掛，
			// 避免臨時樹與原樹共享同一個用例物件。
			var Copy = new TestCase{
				UniqName = Case.UniqName,
				TesterType = Case.TesterType,
				TesteeTypes = [.. Case.TesteeTypes],
				TesteeFnNames = [.. Case.TesteeFnNames],
				FnTest = Case.FnTest,
			};
			Root.Children.Add(new TestNode{
				UniqName = Case.UniqName,
				Data = Copy,
				Parents = [Root],
			});
		}
		return Root;
	}

	[Doc("""
#Sum[執行選中的用例，返回匯總但不打印。]
#Descr[
與 `RunEtPrint` 的差別只有*打印*：
本方法把匯總交回調用方，
適合要自己出報告、或要把多次執行的結果併起來的場合。

執行的是 `MkDetachedTree()` 造出來的扁平臨時樹。

*有失敗也不拋例外*：
本庫默認只跑與報告，
進程的成敗由入口自己判斷。

每次調用都重新造一次臨時樹與執行器，
因此同一個選集可以重複跑。
]
#Params([[執行選項；為空時取「不並行」的默認選項，見 `MakeOpt`],
	[傳給用例函數的參數；`Opt.Arg` 已有值時以 `Opt.Arg` 為準]])
#Rtn[本次執行的匯總統計]
""")]
	public Task<TestRunSummary> Run(OptTestExecutor? Opt = null, obj? Arg = null){
		var Executor = new TreeTestExecutor();
		return Executor.Run([MkDetachedTree()], MakeOpt(Opt, Arg));
	}

	[Doc("""
#Sum[執行選中的用例，並按默認格式打印匯總。]
#Descr[
與 `Run` 走同一條執行路徑，
只在跑完之後多打印一次匯總，
適合寫在測試入口腳本裏直接看結果。

要自己處理結果、或不想讓控制台被測試輸出淹掉時用 `Run`。
]
#Params([[執行選項；為空時取「不並行」的默認選項，見 `MakeOpt`],
	[傳給用例函數的參數；`Opt.Arg` 已有值時以 `Opt.Arg` 為準]])
#Rtn[本次執行的匯總統計]
""")]
	public Task<TestRunSummary> RunEtPrint(OptTestExecutor? Opt = null, obj? Arg = null){
		var Executor = new TreeTestExecutor();
		return Executor.RunEtPrint([MkDetachedTree()], MakeOpt(Opt, Arg));
	}

	[Doc("""
#Sum[組出執行選項；選集預設不並行。]
#Descr[
*選集預設 `IsParallel = false`*：
挑出來的用例原本散在樹上，
彼此之間是否獨立、
原樹的分組順序是否被破壞，
都難以自動判斷；
而且臨時樹是平的（見 `MkDetachedTree`），
原樹的並行設定已經丟失。
因此預設求穩。
確認彼此獨立之後，
調用方可以顯式傳一個 `IsParallel = true` 的選項打開並行。

*會就地修改傳進來的 `Opt`*（只寫 `Arg`）：
要復用同一個 `Opt` 就自己傳副本。
]
#Params([[執行選項；為空時新建一個並行關閉的],
	[參數；只在 `Opt.Arg` 為空時才寫進去，不覆蓋已有值]])
#Rtn[可直接交給執行器的選項]
""")]
	private static OptTestExecutor MakeOpt(OptTestExecutor? Opt, obj? Arg){
		// 默認執行器選項是並行的，
		// 選集要顯式關掉。
		Opt ??= new OptTestExecutor{IsParallel = false};
		// 顯式給的參數不覆蓋調用方已寫進選項的那個。
		Opt.Arg ??= Arg;
		return Opt;
	}
}