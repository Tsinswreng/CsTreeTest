using System.Collections.Concurrent;
using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 單個用例的運行結果。
public sealed class TestCaseRunResult{
	public required int Order { get; set; }
	public required str NodePath { get; set; }
	public required ITestNode Node { get; set; }
	public required ITestCase TestCase { get; set; }
	public required bool IsPassed { get; set; }
	public Exception? Exception { get; set; }
	public TimeSpan Elapsed { get; set; }
}

// 一次執行的匯總統計。
public sealed class TestRunSummary{
	public DateTimeOffset StartedAt { get; set; }
	public DateTimeOffset EndedAt { get; set; }
	public TimeSpan Elapsed => EndedAt - StartedAt;

	public int Total { get; set; }
	public int Passed { get; set; }
	public int Failed { get; set; }

	public IList<TestCaseRunResult> Results { get; set; } = [];
}

// 聲明文件：屬性 + 構造函數聲明（partial method）；實現見 TreeTestExecutor.Impl.cs。
public partial class TestRunFailedException : Exception{
	public TestRunSummary Summary { get; }

	public partial TestRunFailedException(TestRunSummary summary);
}

// 執行選項。
public class OptTestExecutor{
	public bool IsParallel = true;
	public int MaxDegreeOfParallelism = Environment.ProcessorCount;
	public obj? Arg = default;
}

public interface ITestExecutor{
	public Task<TestRunSummary> Run(
		IList<ITestNode> Nodes
		,OptTestExecutor? Opt
	);
}

// 聲明文件：嵌套數據類型 + 方法聲明（partial method）；實現見 TreeTestExecutor.Impl.cs。
public sealed partial class TreeTestExecutor : ITestExecutor{
	/// 一次執行的最小單位：一個用例 + 執行元數據。
	private sealed class WorkItem{
		public required int Order { get; set; }
		public required str NodePath { get; set; }
		public required ITestNode Node { get; set; }
		public required ITestCase TestCase { get; set; }
		public required bool ShouldNotParallelize { get; set; }
	}

	public partial Task<TestRunSummary> Run(
		IList<ITestNode> Nodes,
		OptTestExecutor? Opt
	);

	/// 按 stage 跑完全部 workItem，匯總結果。
	private static partial Task<TestRunSummary> RunCore(
		DateTimeOffset startedAt,
		IList<IList<WorkItem>> workStages,
		ConcurrentBag<TestCaseRunResult> results,
		ParallelOptions parallelOptions,
		obj? arg,
		bool isParallel
	);

	/// 執行單個用例並記錄結果（異常捕獲不中斷）。
	private static partial Task ExecuteItem(
		WorkItem item,
		ConcurrentBag<TestCaseRunResult> results,
		obj? arg
	);

	/// 多個根節點 → 合併成 stage 列表。
	private static partial IList<IList<WorkItem>> CollectWorkStages(IList<ITestNode> nodes);

	/// 單個節點 → stage 列表（Ordered/IsParallelRecursive 決定串/並收斂方式）。
	private static partial IList<IList<WorkItem>> CollectNodeStages(
		ITestNode node,
		str path,
		ref int order,
		bool parentIsParallelRecursive
	);

	/// 確保 stage 列表索引存在（補空 stage）。
	private static partial void EnsureStage(IList<IList<WorkItem>> stages, int index);

	/// （未使用）先行遍歷收集 workItem。
	private static partial void Dfs(
		ITestNode node,
		str path,
		IList<WorkItem> output,
		ref int order
	);
}

// ITestExecutor 的擴展：一行跑測試 / 跑完打印匯總。
// 聲明文件：傳統 this 擴展（調用方寫法與 extension 塊一致）+ partial method 拆分；實現見 TreeTestExecutor.Impl.cs。
public static partial class ExtnTreeTestExecutor{
	[Doc("Run all test cases under current tree root (parallel by default), no default consume")]
	public static partial Task<TestRunSummary> RunTests(
		this ITestExecutor z
		,ITestNode Root
		,obj? Arg = default
		,OptTestExecutor? Opt = default
	);

	[Doc("Run tests then do default consume: print summary and throw when failed")]
	public static partial Task<TestRunSummary> RunEtPrint(
		this ITestExecutor z
		,ITestNode Root
		,obj? Arg = default
		,OptTestExecutor? Opt = default
		,bool ThrowOnFailed = false
	);

	/// 打印失敗用例與匯總（彩色）。
	private static partial void WriteSummary(TestRunSummary summary);
	
	/// 彩色輸出（帶換行）。
	private static partial void WriteWithColor(str text, ConsoleColor color);
	
	/// 彩色輸出（不換行）。
	private static partial void WriteLineWithColor(str text, ConsoleColor color);
}