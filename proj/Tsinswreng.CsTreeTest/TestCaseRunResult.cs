namespace Tsinswreng.CsTreeTest;

[Doc($$"""
單條用例的運行結果。

用例拋出的異常只收進 `{{nameof(Exception)}}`、不往外拋，所以每條跑過的用例都恰好一條結果。
""")]
public sealed class TestCaseRunResult{
	[Doc($$"""
本次執行中的次序，收集階段按樹的先序分配；結果最後按它排序。
""")]
	public required int Order{get;set;}

	[Doc($$"""
名字路徑，例：`Tests/Calculator/Add:正數相加`。只給人看，別當唯一鍵。
""")]
	public required str NodePath{get;set;}

	[Doc($$"""
用例所在的樹節點。
""")]
	public required ITestNode Node{get;set;}

	[Doc($$"""
用例本身。
""")]
	public required ITestCase TestCase{get;set;}

	[Doc($$"""
是否通過：正常返回即真，拋出任何異常即假。
""")]
	public required bool IsPassed{get;set;}

	[Doc($$"""
失敗時的異常；通過時為空。要不要因失敗而失敗，由入口按 `{{nameof(TestRunSummary.Failed)}}` 判斷。
""")]
	public Exception? Exception{get;set;}

	[Doc($$"""
本條用例的耗時，只含用例函數體自身。
""")]
	public TimeSpan Elapsed{get;set;}
}
