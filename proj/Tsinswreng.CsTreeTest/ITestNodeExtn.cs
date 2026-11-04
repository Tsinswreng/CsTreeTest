using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：ExtnITestNode 擴展方法的宣告；實現見 ExtnITestNode.Impl.cs。
[Doc($$"""
#Sum[測試樹節點的擴展方法。]
""")]
public static partial class ExtnITestNode{
	[Doc($$"""
#Sum[往本節點下掛一個普通子節點，並維護父子兩邊的引用。]
#Descr[
給名字時用給的名字；
不給時自動取 ULID，
所以不給名字的節點也可以安全地掛上去。
]
#Params([[父節點，即 `this`；不傳名字時子節點名取 ULID]])
#Rtn[新建的子節點]
""")]
	public static partial ITestNode NewChild(this ITestNode z, str? UniqName = null);

	[Doc($$"""
#Sum[往本節點下掛一個測試類節點。]
#Descr[
測試類類型由泛型參數給。
測試類實例此時還沒建出來，
等測試管理器初始化時才填進 `Tester`。
]
#TParams([測試類類型，須實作 `{{nameof(ITester)}}`])
#Params([[父節點，即 `this`；不傳時節點名取測試類短名]])
#Rtn[新建的測試類節點]
""")]
	public static partial ITesterNode NewTesterChild<T>(this ITestNode z, str? UniqName = null) where T:ITester;

	[Doc($$"""
#Sum[往本節點下掛一個測試類節點，測試類類型由參數給。]
#Descr[
節點名預設取測試類的短名，
使失敗輸出的路徑能直接看出是哪個測試類。
]
#Params([[父節點，即 `this`]，[測試類類型，不能為空]])
#Rtn[新建的測試類節點]
""")]
	public static partial ITesterNode NewTesterChild(this ITestNode z, Type TesterType, str? UniqName = null);

	[Doc($$"""
#Sum[從本節點沿父節點向上走到根，返回從根到本節點這一路各節點的唯一名。]
#Descr[
失敗輸出用本方法打出人看得懂的用例路徑。
只取每個節點的第一個父節點。
]
#Params([[起點，即 `this`]])
#Rtn[從根到本節點的名字列表，含兩端]
""")]
	public static partial IList<str> PathFromRoot(this ITestNode z);

	[Doc($$"""
#Sum[把用例路徑打成字符串，各段以 `/` 相連。]
#Descr[
例：`Tests/Calculator/TestCalculator/Add:正數相加`。
]
#Params([[起點，即 `this`]])
#Rtn[路徑字符串]
""")]
	public static partial str PathString(this ITestNode z);
}
