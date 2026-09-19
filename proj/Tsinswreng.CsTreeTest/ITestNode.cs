using Tsinswreng.CsCore;
using Tsinswreng.CsU128Id;

namespace Tsinswreng.CsTreeTest;

[Doc("""
#Sum[測試樹節點。跑一個節點等於跑它底下的全部用例。]

#Descr[
樹由節點組成。
帶用例的節點是葉子（`Data` 非空），
只作分組的節點不帶用例（`Data` 為空）。

節點有兩個控制執行方式的開關，
它們互相獨立，
不要混用：

+ `Ordered`：
	只管*直接子節點*之間的先後次序，
	不遞歸。
	適合「先插測試數據、再測、最後清理」這種有先後依賴的場景。
+ `IsParallelRecursive`：
	為真時*本節點及其全部遞歸子節點*都不參與並行，
	即只與兄弟依序輪流，
	不與別的分支同時跑。
	本聲明向下繼承。
	適合「本子樹必須獨佔某個共享資源」的場景。
]
""")]
public interface ITestNode{
	[Doc("""
#Sum[節點唯一名。]
#Descr[
同一棵測試樹內不得重複，
重複時建立索引會直接拋異常。

默認取 ULID，
所以節點默認就能掛上去；
需要按名查找（`TestMgr.GetNodeByName`）、
或希望失敗輸出的路徑可讀時，
應顯式指定可讀的名字。
]
""")]
	str UniqName{get;set;}

	[Doc("""
#Sum[該節點攜帶的用例。]
#Descr[
葉節點帶用例；
非葉節點只作分組，此處為空。
]
""")]
	ITestCase? Data{get;set;}

	[Doc("""
#Sum[子節點，順序即聲明順序。]
#Descr[
用 `NewChild` 掛子節點時會自動維護父子兩邊的引用，
不要手工往這個列表裏加。
]
""")]
	IList<ITestNode> Children{get;set;}

	[Doc("""
#Sum[父節點。]
#Descr[
正常用法下一個節點只有一個父節點；
聲明為列表是為了保留圖結構的可能。
向前追溯到根的 `PathFromRoot` 只會取第一個父節點。
]
""")]
	IList<ITestNode> Parents{get;set;}

	[Doc("""
#Sum[為真時，該節點的直接子節點依序執行。]
#Descr[
*不遞歸*：
只約束該節點的直接子節點之間的次序，
再往下的節點不受本聲明影響——
單個子節點內部仍然可以並行。

`Ordered` 不等於「不並行」。
要讓整棵子樹都不與外部同時跑，
用 `IsParallelRecursive`。

例：某節點有三個子節點「插數據、測、清數據」，
把它設為 `true` 即可保證這三步的先後；
這三個子節點各自內部仍然是並行的。
]
""")]
	bool Ordered{get;set;}

	[Doc("""
#Sum[為真時，該節點及其全部遞歸子節點不參與並行。]
#Descr[
即：
本子樹只與自己的兄弟依序輪流，
不與別的分支同時跑。
本聲明*向下繼承*，
標在最外層即可約束整棵子樹。

默認為 `false`。
與 `Ordered` 的區別：
`Ordered` 管的是「直接子節點之間的次序」，
本開關管的是「本子樹能不能與別的分支同時跑」，
兩者可以同時使用。
]
""")]
	bool IsParallelRecursive{get;set;}
}

[Doc("""
#Sum[掛測試類的節點：樹節點之外，還記着是哪個測試類註冊了它。]
#Descr[
由 `NewTesterChild` 建立。
一般不用直接 new 本接口的實現類。
]
""")]
public interface ITesterNode:ITestNode{
	[Doc("""
#Sum[註冊本節點的測試類類型。]
#Descr[
由 `NewTesterChild` 寫入。
索引與篩選（按測試類找節點）就是靠它。
]
""")]
	Type? TesterType{get;set;}

	[Doc("""
#Sum[測試類實例。]
#Descr[
框架不經過依賴容器，
測試類由測試管理器在 `Init` 時自行 `new` 出來並記在此處，
以便需要時能從樹反查到測試類對象。
]
""")]
	ITester? Tester{get;set;}
}

[Doc("""
#Sum[測試樹節點的默認實現：純數據，無額外行爲。]
""")]
public class TestNode : ITestNode{
	[Doc(@"#Sum[唯一名。默認取 ULID；要按名查找時應顯式指定可讀名字。]")]
	public str UniqName{get;set;} = U128Id.NewUlid().ToLow64Base();

	[Doc(@"#Sum[用例。非葉節點為空。]")]
	public ITestCase? Data{get;set;}

	[Doc(@"#Sum[子節點，順序即聲明順序。]")]
	public IList<ITestNode> Children{get;set;} = [];

	[Doc(@"#Sum[父節點。正常用法下只有一個。]")]
	public IList<ITestNode> Parents{get;set;} = [];

	[Doc(@"#Sum[直接子節點是否依序執行，不遞歸。默認 false。]")]
	public bool Ordered{get;set;} = false;

	[Doc(@"#Sum[本子樹是否不參與並行，向下繼承。默認 false。]")]
	public bool IsParallelRecursive{get;set;} = false;
}

[Doc("""
#Sum[掛測試類的節點。]
#Descr[
在 `TestNode` 之上多記一個測試類類型，
以及該測試類的實例。
]
""")]
public class TesterNode : TestNode, ITesterNode{
	[Doc(@"#Sum[掛在本節點的測試類類型。]")]
	public Type? TesterType{get;set;}

	[Doc(@"#Sum[測試類實例，由測試管理器在 Init 時填入。]")]
	public ITester? Tester{get;set;}
}

[Doc("""
#Sum[測試樹節點的擴展方法。]
""")]
public static partial class ExtnITestNode{
	[Doc("""
#Sum[往本節點下掛一個普通子節點，並維護父子兩邊的引用。]
#Descr[
給名字時用給的名字；
不給時自動取 ULID，
所以不給名字的節點也可以安全地掛上去。
]
#Params([[父節點，即 `this`；不傳名字時子節點名取 ULID]])
#Rtn[新建的子節點]
""")]
	public static ITestNode NewChild(this ITestNode z, str? UniqName = null){
		var R = new TestNode{
			UniqName = UniqName ?? U128Id.NewUlid().ToLow64Base(),
		};
		z.Children.Add(R);
		R.Parents.Add(z);
		return R;
	}

	[Doc("""
#Sum[往本節點下掛一個測試類節點。]
#Descr[
測試類類型由泛型參數給。
測試類實例此時還沒建出來，
等測試管理器初始化時才填進 `Tester`。
]
#TParams([測試類類型])
#Params([[父節點，即 `this`；不傳時節點名取測試類短名]])
#Rtn[新建的測試類節點]
""")]
	public static ITesterNode NewTesterChild<T>(this ITestNode z, str? UniqName = null) where T:ITester{
		return z.NewTesterChild(typeof(T), UniqName);
	}

	[Doc("""
#Sum[往本節點下掛一個測試類節點，測試類類型由參數給。]
#Descr[
節點名預設取測試類的短名，
使失敗輸出的路徑能直接看出是哪個測試類。
]
#Params([[父節點，即 `this`]，[測試類類型，不能為空]])
#Rtn[新建的測試類節點]
""")]
	public static ITesterNode NewTesterChild(this ITestNode z, Type TesterType, str? UniqName = null){
		ArgumentNullException.ThrowIfNull(TesterType);
		var R = new TesterNode{
			UniqName = UniqName ?? TesterType.Name,
			TesterType = TesterType,
		};
		z.Children.Add(R);
		R.Parents.Add(z);
		return R;
	}

	[Doc("""
#Sum[從本節點沿父節點向上走到根，返回從根到本節點這一路各節點的唯一名。]
#Descr[
失敗輸出用本方法打出人看得懂的用例路徑。
只取每個節點的第一個父節點。
]
#Params([[起點，即 `this`]])
#Rtn[從根到本節點的名字列表，含兩端]
""")]
	public static IList<str> PathFromRoot(this ITestNode z){
		var R = new List<str>();
		ITestNode? Cur = z;
		// 樹正常時不會成環；
		// 加步數上限是為了防止使用者手工接出環之後死循環。
		var Guard = 0;
		while(Cur is not null && Guard++ < 1000){
			R.Add(Cur.UniqName);
			Cur = Cur.Parents.Count > 0 ? Cur.Parents[0] : null;
		}
		R.Reverse();
		return R;
	}

	[Doc("""
#Sum[把用例路徑打成字符串，各段以 `/` 相連。]
#Descr[
例：`Tests/Calculator/TestCalculator/Add:正數相加`。
]
#Params([[起點，即 `this`]])
#Rtn[路徑字符串]
""")]
	public static str PathString(this ITestNode z){
		return string.Join("/", z.PathFromRoot());
	}
}