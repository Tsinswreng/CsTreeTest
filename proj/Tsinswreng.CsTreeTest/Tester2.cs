using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：Tester2 的成員宣告；實現見 Tester2.Impl.cs（本輪只寫聲明）。
[Doc($$"""
`{{nameof(ITester2)}}` 的默認實現：子類只需重寫 `{{nameof(Init)}}` 聲明自己那棵子樹。

```cs
// 一個被測類的測試類：往自己的節點上掛用例
public class TestRepo : Tester2{
	public override void Init(){
		TestNode.Ordered = true;
		TesteeTypes = [typeof(IRepo<TestKv, IdTestKv>)];
		TesteeFnNames = [nameof(IRepo<TestKv, IdTestKv>.OrdAdd)];
		AddTestCase("插入三筆", async O => { return NIL; });
	}
}

// 程序集的組裝器：往自己的節點上掛測試類，類型由泛型參數給
public class CsSqlTester : Tester2{
	public override void Init(){
		AddTester<TestRepo>();
	}
}
```

父方掛一個測試類用 `{{nameof(AddTester)}}`：節點由父方建，`TesterType` 也由父方寫，子方的 `{{nameof(ITester2.Init)}}` 只負責填內容。
""")]
public abstract partial class Tester2 : ITester2{
	[Doc($$"""
	本測試類的子樹根節點。

	#See[{{nameof(ITester2.TestNode)}}]
	""")]
	public ITestNode TestNode{get;set;} = null!;

	[Doc($$"""
	#See[{{nameof(ITester2.Inited)}}]
	""")]
	public bool Inited{get;set;}

	[Doc($$"""
	本測試類的用例所測的被測類型。

	`{{nameof(AddTestCase)}}` 按調用時的值快照給用例，所以在此一次性聲明，適用於本測試類的全部用例。
	""")]
	public IList<Type> TesteeTypes{get;set;} = [];

	[Doc($$"""
	本測試類的用例所測的被測函數名。

	`{{nameof(AddTestCase)}}` 按調用時的值快照給用例；
	同一個用例檔要分別測幾個被測函數時，逐條用例改這個字段即可。
	""")]
	public IList<str> TesteeFnNames{get;set;} = [];

	[Doc($$"""
	#See[{{nameof(ITester2.Init)}}]
	""")]
	public abstract void Init();

	[Doc($$"""
	把一個子測試類掛到本節點下，類型由泛型參數給。

	本方法負責建子節點：`TesterType` 取 `{{nameof(TTester)}}`，`{{nameof(ITestNode.UniqName)}}` 默認取類型短名。
	節點建好後交給子方，再由子方的 `{{nameof(ITester2.Init)}}` 填內容。
	同一個實例只能掛一次，重複掛拋 `InvalidOperationException`。

	#TPrm[{{nameof(TTester)}}][子測試類類型，須有無參構造子]
	""")]
	public partial void AddTester<TTester>() where TTester: ITester2, new();

	//TswgNote 記錄 TesterType 信息
	[Doc($$"""
	同上，實例與類型都由調用方給，用於運行時才知道測試類型的場合。

	#Prm[{{nameof(Tester)}}][子測試類實例]
	#Prm[{{nameof(TesterType)}}][子測試類類型，不能為空；寫進子節點的 `TesterType`，按測試類篩選靠它]
	""")]
	public partial void AddTester(ITester2 Tester, Type TesterType);

	[Doc($$"""
	往本節點下掛一條用例，用例節點的唯一名即 `{{nameof(UniqName)}}`。

	用例的元數據取調用時的 `{{nameof(TesteeTypes)}}` 與 `{{nameof(TesteeFnNames)}}`，
	所屬測試類取本節點的 `TesterType`。

	#Prm[{{nameof(UniqName)}}][用例名，全樹唯一；要能看出所測行為]
	#Prm[{{nameof(FnTest)}}][用例本體]
	""")]
	public partial void AddTestCase(str UniqName, FnTest FnTest);
}
