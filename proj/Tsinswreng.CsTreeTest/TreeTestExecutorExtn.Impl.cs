using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public static partial class TreeTestExecutorExtn{
	public static partial Task<TestRunSummary> RunTests(
		this ITestExecutor z
		,ITestNode Root
		,obj? Arg
		,OptTestExecutor? Opt
	){
		ArgumentNullException.ThrowIfNull(Root);
		Opt ??= new OptTestExecutor();
		Opt.Arg ??= Arg;
		return z.Run([Root], Opt);
	}

	public static partial Task<TestRunSummary> RunEtPrint(
		this ITestExecutor z
		,ITestNode Root
		,obj? Arg
		,OptTestExecutor? Opt
		,bool ThrowOnFailed
	){
		ArgumentNullException.ThrowIfNull(Root);
		return z.RunEtPrint([Root], Arg, Opt, ThrowOnFailed);
	}

	public static partial Task<TestRunSummary> RunTests(
		this ITestExecutor z
		,IList<ITestNode> Nodes
		,obj? Arg
		,OptTestExecutor? Opt
	){
		ArgumentNullException.ThrowIfNull(Nodes);
		Opt ??= new OptTestExecutor();
		Opt.Arg ??= Arg;
		return z.Run(Nodes, Opt);
	}

	public static async partial Task<TestRunSummary> RunEtPrint(
		this ITestExecutor z
		,IList<ITestNode> Nodes
		,obj? Arg
		,OptTestExecutor? Opt
		,bool ThrowOnFailed
	){
		ArgumentNullException.ThrowIfNull(Nodes);
		Opt ??= new OptTestExecutor();
		Opt.Arg ??= Arg;
		// step 1: 跑。
		var Summary = await z.Run(Nodes, Opt);
		// step 2: 不管成敗都先打印，這樣拋例外時人已經看到逐條原因。
		WriteSummary(Summary);
		// step 3: 只有明確要求時才因失敗而拋。
		if(ThrowOnFailed && Summary.Failed > 0){
			throw new TestRunFailedException(Summary);
		}
		return Summary;
	}

	private static partial void WriteSummary(TestRunSummary Summary){
		// step 1: 逐條打印失敗用例，通過的跳過。
		foreach(var Result in Summary.Results.OrderBy(x => x.Order)){
			if(Result.IsPassed){
				continue;
			}
			// 測試類類型可能為空（用例不是經註冊器造的），此時給個佔位名。
			var TesterName = Result.TestCase.TesterType?.Name ?? "UnknownTester";
			WriteWithColor("[失敗]", ConsoleColor.Red);
			Console.WriteLine($" {TesterName}/{Result.TestCase.UniqName} （{Result.Elapsed}）");
			Console.WriteLine($"    用例路徑：{Result.NodePath}");
			if(Result.Exception is not null){
				WriteLineWithColor(Result.Exception.ToString(), ConsoleColor.DarkRed);
			}
		}
		// step 2: 打印匯總。
		Console.WriteLine();
		WriteWithColor($"[{nameof(CsTreeTest)}]", ConsoleColor.Cyan);
		Console.WriteLine();
		Console.WriteLine($"總數={Summary.Total}");
		// 通過數非零才標綠，避免 0 通過時誤導性地顯示綠色。
		WriteWithColor(
			$"通過={Summary.Passed}"
			,Summary.Passed > 0 ? ConsoleColor.Green : Console.ForegroundColor
		);
		Console.WriteLine();
		// 同上：有失敗才標紅。
		WriteWithColor(
			$"失敗={Summary.Failed}"
			,Summary.Failed > 0 ? ConsoleColor.Red : Console.ForegroundColor
		);
		Console.WriteLine();
		Console.WriteLine($"耗時={Summary.Elapsed}");
		Console.WriteLine();
	}

	private static partial void WriteWithColor(str Text, ConsoleColor Color){
		var OldColor = Console.ForegroundColor;
		Console.ForegroundColor = Color;
		Console.Write(Text);
		Console.ForegroundColor = OldColor;
	}

	private static partial void WriteLineWithColor(str Text, ConsoleColor Color){
		var OldColor = Console.ForegroundColor;
		Console.ForegroundColor = Color;
		Console.WriteLine(Text);
		Console.ForegroundColor = OldColor;
	}
}
