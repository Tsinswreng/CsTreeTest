using System.Collections.Concurrent;

using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：TreeTestExecutor 的成員宣告；實現見 TreeTestExecutor.Impl.cs。
[Doc($$"""
測試執行器的默認實現：按層分階段推進，階段之間有屏障。

收集階段把樹攤平成一串階段：同一階段內的項互不依賴、可同時跑，下一階段必須等本階段跑完——
「先插數據、再測、最後清理」就是靠這個屏障成立的。
階段內先跑可並行項（受 `{{nameof(OptTestExecutor.MaxDegreeOfParallelism)}}` 限制），再依序跑標了不並行的項。
失敗不中斷：用例異常收進 `{{nameof(TestCaseRunResult.Exception)}}`；結果最後按 `{{nameof(TestCaseRunResult.Order)}}` 排好再返回。
""")]
public sealed partial class TreeTestExecutor : ITestExecutor{
	[Doc($$"""
一次執行的最小單位：一條用例 + 收集階段算好的執行元數據。跑階段因此不必再碰樹。
""")]
	private sealed class WorkItem{
		[Doc($$"""
先序序號，由收集階段共享的遞增計數器分配。
""")]
		public required int Order{get;set;}

		[Doc($$"""
名字路徑，收集時由 `{{nameof(ITestNodeExtn.PathString)}}` 算好。
""")]
		public required str NodePath{get;set;}

		[Doc($$"""
用例所在的樹節點。
""")]
		public required ITestNode Node{get;set;}

		[Doc($$"""
要跑的那條用例。
""")]
		public required ITestCase TestCase{get;set;}

		[Doc($$"""
本項是否不參與並行；繼承已經算進去了。
""")]
		public required bool ShouldNotParallelize{get;set;}
	}

	[Doc($$"""
跑一組節點：收集 → 跑 → 匯總。

參數兜底只在這個入口：`{{nameof(Nodes)}}` 為空引用直接拋，`{{nameof(Opt)}}` 為空用默認值。

#Rtn[本次執行的匯總]
""")]
	public partial Task<TestRunSummary> Run(
		IList<ITestNode> Nodes,
		OptTestExecutor? Opt
	);

	[Doc($$"""
按階段跑；`{{nameof(OptTestExecutor.IsParallel)}}` 為假時整棵樹單線程按先序跑。

同一階段分兩批：先並行項，再不可並行項（「不可並行」＝不與其它項同時跑）。最後按 `{{nameof(TestCaseRunResult.Order)}}` 排序再統計。

#Rtn[本次執行的匯總]
""")]
	private static partial Task<TestRunSummary> RunCore(
		DateTimeOffset StartedAt,
		IList<IList<WorkItem>> WorkStages,
		ConcurrentBag<TestCaseRunResult> Results,
		ParallelOptions ParallelOptions,
		obj? Arg,
		bool IsParallel
	);

	[Doc($$"""
跑單條用例，成功失敗都收成一條結果。
用例拋出的異常只收下、不外拋，否則會帶走同批並行的其它用例。
""")]
	private static partial Task ExecuteItem(
		WorkItem Item,
		ConcurrentBag<TestCaseRunResult> Results,
		obj? Arg
	);

	[Doc($$"""
多個根各自攤平，再按階段序號合併，使多根之間也共享階段屏障。
`{{nameof(TestCaseRunResult.Order)}}` 是跨根共享的計數器。
#Rtn[階段列表，下標即階段序號]
""")]
	private static partial IList<IList<WorkItem>> CollectWorkStages(IList<ITestNode> Nodes);

	[Doc($$"""
收集單個子樹，攤平成階段列表。

葉子＝單階段單項；`{{nameof(ITestNode.Ordered)}}` 子節點首尾相接成串行鏈；其它按階段序號合併，同層因此可並行。
`{{nameof(ITestNode.NonParallelizable)}}` 按「父繼承或自身」往下傳。

#Rtn[本子樹的階段列表]
""")]
	private static partial IList<IList<WorkItem>> CollectNodeStages(
		ITestNode Node,
		str Path,
		ref int Order,
		bool ParentNonParallelizable
	);

	[Doc($$"""
保證階段列表至少有 `Index + 1` 個階段，不足時補空階段。
""")]
	private static partial void EnsureStage(IList<IList<WorkItem>> Stages, int Index);
}
