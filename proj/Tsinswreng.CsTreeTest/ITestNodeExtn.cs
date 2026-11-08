using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：ITestNodeExtn 擴展方法的宣告；實現見 ITestNodeExtn.Impl.cs。
[Doc($$"""
測試樹節點的擴展方法：掛節點、遍歷、篩選。
""")]
public static partial class ITestNodeExtn{
	[Doc($$"""
	掛一個普通子節點，並維護父子兩邊的引用。

#Prm[{{nameof(UniqName)}}][為空時取 UUID]
#Rtn[新建的子節點]
""")]
	public static partial ITestNode NewChild(this ITestNode z, str? UniqName = null);

	[Doc($$"""
	掛一個測試類節點。

	測試類實例此時還沒建，`{{nameof(ITester.RegisterTestsInto)}}` 之後才填進 `{{nameof(ITesterNode.Tester)}}`。

#TPrm[{{nameof(T)}}][測試類類型]
#Prm[{{nameof(UniqName)}}][為空時取測試類短名]
#Rtn[新建的測試類節點]
""")]
	public static partial ITesterNode NewTesterChild<T>(this ITestNode z, str? UniqName = null) where T:ITester;

	[Doc($$"""
	同前，測試類類型由參數給。

#Prm[{{nameof(TesterType)}}][不能為空]
#Rtn[新建的測試類節點]
""")]
	public static partial ITesterNode NewTesterChild(this ITestNode z, Type TesterType, str? UniqName = null);

	[Doc($$"""
	往本節點下掛一條用例。

	用例節點的唯一名即 `{{nameof(UniqName)}}`；用例本體包成 `{{nameof(TestCase)}}` 放進新節點的 `{{nameof(ITestNode.Data)}}`。
	父節點可以是普通節點，也可以是測試類節點或別的用例節點，本方法只管掛。

#Prm[{{nameof(UniqName)}}][用例名，全樹唯一；要能看出所測行為]
#Prm[{{nameof(FnTest)}}][用例本體]
#Rtn[結果，取出新建的用例節點]
""")]
	public static partial IResAddTestCase AddTestCase(this ITestNode z, str UniqName, FnTest FnTest);

	[Doc($$"""
	先序遍歷本節點及其全部子節點，含自身。不防環。
""")]
	public static partial IEnumerable<ITestNode> Walk(this ITestNode z);

	[Doc($$"""
	先序遍歷，只取帶用例的節點（`{{nameof(ITestNode.Data)}}` 非空）。
""")]
	public static partial IEnumerable<ITestNode> WalkCases(this ITestNode z);

	[Doc($$"""
	從本節點沿父節點上溯到根，返回這一路的唯一名。

#Rtn[從根到本節點的名字列表，含兩端]
""")]
	public static partial IList<str> PathFromRoot(this ITestNode z);

	[Doc($$"""
	把用例路徑打成字符串，各段以 `/` 相連。

	例：`Tests/Calculator/Add:正數相加`。
""")]
	public static partial str PathString(this ITestNode z);
}
