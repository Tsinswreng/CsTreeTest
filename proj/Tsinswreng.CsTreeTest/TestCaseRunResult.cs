namespace Tsinswreng.CsTreeTest;

[Doc($$"""
#Sum[單條用例的運行結果。]
#Descr[
一次執行會為每條被跑到的用例造一個本對象。

*失敗不中斷*：
單條用例拋出的異常被收進 `Exception` 字段，
不會往外拋、也不會影響同批的其它用例，
所以每條被跑到的用例在 `{{nameof(TestRunSummary.Results)}}` 裏一定有且只有一條結果。
]
""")]
public sealed class TestCaseRunResult{
	[Doc($$"""
#Sum[該用例在本次執行中的次序。]
#Descr[
由*收集階段*按先序（父在前、同層按聲明序）逐項分配，
從 0 開始單調遞增，跨多個根節點也共用同一個計數器。

*為甚麼要這個字段*：
用例是並行跑的，完成順序隨機；
`{{nameof(TreeTestExecutor)}}` 的執行流程最後按本字段排序後才返回，
使用者看到的 `{{nameof(TestRunSummary.Results)}}` 因此與「樹的先序」一致，
兩次執行的輸出才可穩定對比。
]
""")]
	public required int Order{get;set;}

	[Doc($$"""
#Sum[該用例在樹上的位置，是一個*名字路徑*。]
#Descr[
由各祖先節點（含本節點）的唯一名以 `/` 拼成，例如
`Tests/Calculator/TestCalculator/Add:正數相加`。
在收集階段由 `{{nameof(ExtnITestNode.PathString)}}` 沿父節點上溯算好。

*為甚麼用名字路徑*：失敗輸出要給人看，
名字路徑一眼就能看出是哪個測試類的哪條用例，
比一串節點標識可讀得多。

*已知取捨*：這是名字路徑而非節點標識路徑，
同名節點下的路徑可能不唯一。
本字段只用於人看，
不要拿它當唯一鍵去比對用例；
要拿節點請用 `Node`。
]
""")]
	public required str NodePath{get;set;}

	[Doc($$"""
#Sum[該用例所在的樹節點。]
#Descr[
`{{nameof(TestCaseRunResult.NodePath)}}` 就是從本節點沿父節點上溯算出來的，
需要節點上的其它信息（`{{nameof(ITestNode.Children)}}`、`{{nameof(ITestNode.Parents)}}`、`Data` 等）時從這裏取。
]
""")]
	public required ITestNode Node{get;set;}

	[Doc($$"""
#Sum[該用例本身。]
#Descr[
失敗輸出時從它取 `{{nameof(ITesterNode.TesterType)}}` 與 `{{nameof(ITestNode.UniqName)}}`（見 `{{nameof(TreeTestExecutor)}}` 的失敗輸出），
所以結果對象不必再另存一份用例名。
]
""")]
	public required ITestCase TestCase{get;set;}

	[Doc(@$"#Sum[是否通過。用例函數正常返回即為真；拋出任何異常即為假。]")]
	public required bool IsPassed{get;set;}

	[Doc($$"""
#Sum[失敗時的例外；通過時為空。]
#Descr[
*用例拋出的異常只被收下、不往外拋*，
這是「失敗不中斷其餘用例」的落點。

是否要有失敗就讓進程失敗，
由入口按 `{{nameof(TestRunSummary.Failed)}}` 自行判斷；
本庫自己不做這個決定（見 `{{nameof(ExtnTreeTestExecutor.RunEtPrint)}}` 的 `ThrowOnFailed`）。
]
""")]
	public Exception? Exception{get;set;}

	[Doc(@$"#Sum[本條用例的耗時。只含用例函數體自身的執行時間，不含收集與匯總的時間。]")]
	public TimeSpan Elapsed{get;set;}
}
