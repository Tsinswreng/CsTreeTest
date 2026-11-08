using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：Tester 的成員宣告；實現見 Tester.Impl.cs。
[Doc($$"""
`{{nameof(ITester)}}` 的默認實現：提供節點與初始化狀態的存儲，子類重寫 `{{nameof(Init)}}` 即可。

```cs
public class TestRepo : Tester{
	public override void Init(){
		TestNode.Ordered = true;
		AddTestCase("插入三筆", async O => { return NIL; });
	}
}
```

同時把舊入口 `{{nameof(RegisterTestsInto)}}` 接到新流程上，
所以舊的 `{{nameof(TestMgr)}}` 也能掛用新寫法寫的測試類。
""")]
public abstract partial class Tester : ITester{
	[Doc($$"""
	#See[{{nameof(ITester.TestNode)}}]
	""")]
	public ITestNode TestNode{get;set;} = null!;

	[Doc($$"""
	#See[{{nameof(ITester.Inited)}}]
	""")]
	public bool Inited{get;set;}

	[Doc($$"""
	#See[{{nameof(ITester.Init)}}]
	""")]
	public abstract void Init();

	[Doc($$"""
	#See[{{nameof(ITester.AddTestCase)}}]
	""")]
	public partial IResAddTestCase AddTestCase(str UniqName, FnTest FnTest);

	[Doc($$"""
	舊入口的橋：把節點交給本測試類，再走新流程填內容。

	只為讓 `{{nameof(TestMgr)}}` 那條老路徑也能掛新寫法的測試類；新代碼請直接走 `{{nameof(Init)}}`。
	""")]
	[Obsolete($$"""
	改用 `{{nameof(Init)}}` 與 `{{nameof(TestNode)}}`。
	""")]
	public partial ITestNode RegisterTestsInto(ITestNode? Node);
}
