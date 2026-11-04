using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：ITestNodeExtn 擴展方法的宣告；實現見 ITestNodeExtn.Impl.cs。
[Doc($$"""
測試樹節點的擴展方法。
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

測試類實例此時還沒建，`{{nameof(TestMgr.Init)}}` 時才填進 `{{nameof(ITesterNode.Tester)}}`。

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
