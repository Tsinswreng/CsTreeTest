using System.Collections.Concurrent;
using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

public sealed class TestCaseRunResult{
	public required int Order { get; set; }
	public required str NodePath { get; set; }
	public required ITestNode Node { get; set; }
	public required ITestCase TestCase { get; set; }
	public required bool IsPassed { get; set; }
	public Exception? Exception { get; set; }
	public TimeSpan Elapsed { get; set; }
}

public sealed class TestRunSummary{
	public DateTimeOffset StartedAt { get; set; }
	public DateTimeOffset EndedAt { get; set; }
	public TimeSpan Elapsed => EndedAt - StartedAt;

	public int Total { get; set; }
	public int Passed { get; set; }
	public int Failed { get; set; }

	public IList<TestCaseRunResult> Results { get; set; } = [];
}

public sealed class TestRunFailedException : Exception{
	public TestRunSummary Summary { get; }

	public TestRunFailedException(TestRunSummary summary)
		: base($"Test run failed. Total={summary.Total}, Passed={summary.Passed}, Failed={summary.Failed}")
	{
		Summary = summary;
	}
}

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

public sealed class TreeTestExecutor : ITestExecutor{
	private sealed class WorkItem{
		public required int Order { get; set; }
		public required str NodePath { get; set; }
		public required ITestNode Node { get; set; }
		public required ITestCase TestCase { get; set; }
		public required bool ShouldNotParallelize { get; set; }
	}

	public Task<TestRunSummary> Run(
		IList<ITestNode> Nodes,
		OptTestExecutor? Opt
	){
		ArgumentNullException.ThrowIfNull(Nodes);
		Opt ??= new OptTestExecutor();

		var startedAt = DateTimeOffset.Now;
		var workStages = CollectWorkStages(Nodes);
		var results = new ConcurrentBag<TestCaseRunResult>();
		var parallelOptions = new ParallelOptions{
			MaxDegreeOfParallelism = Opt.MaxDegreeOfParallelism <= 0
				? Environment.ProcessorCount
				: Opt.MaxDegreeOfParallelism,
		};

		return RunCore(startedAt, workStages, results, parallelOptions, Opt.Arg, Opt.IsParallel);
	}

	private static async Task<TestRunSummary> RunCore(
		DateTimeOffset startedAt,
		IList<IList<WorkItem>> workStages,
		ConcurrentBag<TestCaseRunResult> results,
		ParallelOptions parallelOptions,
		obj? arg,
		bool isParallel
	){
		if(isParallel){
			foreach(var stage in workStages){
				var parallelItems = stage.Where(x => !x.ShouldNotParallelize).ToList();
				var sequentialItems = stage.Where(x => x.ShouldNotParallelize).ToList();
				
				if(parallelItems.Count > 0){
					await Parallel.ForEachAsync(parallelItems, parallelOptions, async (item, _) => {
						await ExecuteItem(item, results, arg);
					});
				}
				
				foreach(var item in sequentialItems){
					await ExecuteItem(item, results, arg);
				}
			}
		}
		else{
			foreach(var stage in workStages){
				foreach(var item in stage){
					parallelOptions.CancellationToken.ThrowIfCancellationRequested();
					await ExecuteItem(item, results, arg);
				}
			}
		}

		var ordered = results.OrderBy(x => x.Order).ToList();
		var passed = ordered.Count(x => x.IsPassed);
		var failed = ordered.Count - passed;

		return new TestRunSummary{
			StartedAt = startedAt,
			EndedAt = DateTimeOffset.Now,
			Total = ordered.Count,
			Passed = passed,
			Failed = failed,
			Results = ordered,
		};
	}

	private static async Task ExecuteItem(
		WorkItem item,
		ConcurrentBag<TestCaseRunResult> results,
		obj? arg
	){
		var begin = DateTimeOffset.Now;
		try{
			await item.TestCase.FnTest(arg);
			results.Add(new TestCaseRunResult{
				Order = item.Order,
				NodePath = item.NodePath,
				Node = item.Node,
				TestCase = item.TestCase,
				IsPassed = true,
				Exception = null,
				Elapsed = DateTimeOffset.Now - begin,
			});
		}
		catch(Exception ex){
			results.Add(new TestCaseRunResult{
				Order = item.Order,
				NodePath = item.NodePath,
				Node = item.Node,
				TestCase = item.TestCase,
				IsPassed = false,
				Exception = ex,
				Elapsed = DateTimeOffset.Now - begin,
			});
		}
	}

	private static IList<IList<WorkItem>> CollectWorkStages(IList<ITestNode> nodes){
		var order = 0;
		var output = new List<IList<WorkItem>>();
		var nodeStages = new List<IList<WorkItem>>();
		for(var i = 0; i < nodes.Count; i++){
			nodeStages.AddRange(CollectNodeStages(nodes[i], $"{i}", ref order, false));
		}

		for(var i = 0; i < nodeStages.Count; i++){
			EnsureStage(output, i);
			for(var j = 0; j < nodeStages[i].Count; j++){
				output[i].Add(nodeStages[i][j]);
			}
		}

		return output;
	}

	private static IList<IList<WorkItem>> CollectNodeStages(
		ITestNode node,
		str path,
		ref int order,
		bool parentIsParallelRecursive
	){
		var output = new List<IList<WorkItem>>();

		if(node.Data is ITestCase testCase){

			output.Add([
				new WorkItem{
					Order = order++,
					NodePath = path,
					Node = node,
					TestCase = testCase,
					ShouldNotParallelize = parentIsParallelRecursive || node.IsParallelRecursive,
				}
			]);
		}
		if(node.Children.Count == 0){
			return output;
		}

		// If parent has IsParallelRecursive or current node has it, force sequential execution
		var shouldOrder = node.Ordered || node.IsParallelRecursive || parentIsParallelRecursive;
		// Pass down the IsParallelRecursive flag to children
		var childIsParallelRecursive = node.IsParallelRecursive || parentIsParallelRecursive;
		
		if(shouldOrder){
			for(var i = 0; i < node.Children.Count; i++){
				var childStages = CollectNodeStages(node.Children[i], $"{path}/{i}", ref order, childIsParallelRecursive);
				for(var j = 0; j < childStages.Count; j++){
					output.Add(childStages[j]);
				}
			}
		}
		else{
			var mergedChildStages = new List<IList<WorkItem>>();
			for(var i = 0; i < node.Children.Count; i++){
				var childStages = CollectNodeStages(node.Children[i], $"{path}/{i}", ref order, childIsParallelRecursive);
				for(var j = 0; j < childStages.Count; j++){
					EnsureStage(mergedChildStages, j);
					for(var k = 0; k < childStages[j].Count; k++){
						mergedChildStages[j].Add(childStages[j][k]);
					}
				}
			}

			for(var i = 0; i < mergedChildStages.Count; i++){
				output.Add(mergedChildStages[i]);
			}
		}

		return output;
	}

	private static void EnsureStage(IList<IList<WorkItem>> stages, int index){
		while(stages.Count <= index){
			stages.Add([]);
		}
	}

	private static void Dfs(
		ITestNode node,
		str path,
		IList<WorkItem> output,
		ref int order
	){
		if(node.Data is ITestCase testCase){

			output.Add(new WorkItem{
				Order = order++,
				NodePath = path,
				Node = node,
				TestCase = testCase,
				ShouldNotParallelize = false,
			});
		}
		for(var i = 0; i < node.Children.Count; i++){
			Dfs(node.Children[i], $"{path}/{i}", output, ref order);
		}
	}
}

public static class ExtnTreeTestExecutor{
	extension(ITestExecutor z){
		[Doc("Run all test cases under current tree root (parallel by default), no default consume")]
		public Task<TestRunSummary> RunTests(
			ITestNode Root,
			obj? Arg = default,
			OptTestExecutor? Opt = default
		){
			Opt ??= new OptTestExecutor();
			Opt.Arg = Arg;
			return z.Run([Root], Opt);
		}

		[Doc("Run tests then do default consume: print summary and throw when failed")]
		public async Task<TestRunSummary> RunEtPrint(
			ITestNode Root,
			obj? Arg = default,
			OptTestExecutor? Opt = default,
			bool ThrowOnFailed = false
		){
			Opt ??= new OptTestExecutor();
			Opt.Arg = Arg;
			var summary = await z.Run([Root], Opt);
			WriteSummary(summary);
			if(ThrowOnFailed && summary.Failed > 0){
				throw new TestRunFailedException(summary);
			}
			return summary;
		}
	}

	private static void WriteSummary(TestRunSummary summary){
		foreach(var result in summary.Results.OrderBy(x => x.Order)){
			if(result.IsPassed){
				continue;
			}
			var testerName = result.TestCase.TesterType?.Name ?? "UnknownTester";
			WriteWithColor("[FAIL]", ConsoleColor.Red);
			Console.WriteLine($" {testerName}/{result.TestCase.UniqName} ({result.Elapsed})");
			if(result.Exception is not null){
				WriteLineWithColor(result.Exception.ToString(), ConsoleColor.DarkRed);
			}
		}
		System.Console.WriteLine();
		WriteWithColor($"[{nameof(CsTreeTest)}]", ConsoleColor.Cyan);
		System.Console.WriteLine();
		Console.Write($"Total={summary.Total}");
		System.Console.WriteLine();
		WriteWithColor($"Passed={summary.Passed}", summary.Passed > 0 ? ConsoleColor.Green : Console.ForegroundColor);
		System.Console.WriteLine();
		WriteWithColor($"Failed={summary.Failed}", summary.Failed > 0 ? ConsoleColor.Red : Console.ForegroundColor);
		System.Console.WriteLine();
		Console.WriteLine($"Elapsed={summary.Elapsed}");
		System.Console.WriteLine();
	}
	

	private static void WriteWithColor(str text, ConsoleColor color){
		var oldColor = Console.ForegroundColor;
		Console.ForegroundColor = color;
		Console.Write(text);
		Console.ForegroundColor = oldColor;
	}

	private static void WriteLineWithColor(str text, ConsoleColor color){
		var oldColor = Console.ForegroundColor;
		Console.ForegroundColor = color;
		Console.WriteLine(text);
		Console.ForegroundColor = oldColor;
	}
}
