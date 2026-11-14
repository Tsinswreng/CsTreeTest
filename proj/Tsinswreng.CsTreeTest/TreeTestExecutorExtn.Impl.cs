using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public static partial class TreeTestExecutorExtn{
	public static Action<str?> Println = Console.WriteLine;
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
			WriteWithColor("[FAILED]", ConsoleColor.Red);
			Println($" {TesterName}/{Result.TestCase.UniqName} ({Result.Elapsed})");
			Println($"    Case path: {Result.NodePath}");
			if(Result.Exception is not null){
				WriteLineWithColor(Result.Exception.ToString(), ConsoleColor.DarkRed);
			}
		}
		// step 2: 打印匯總。
		Println(null);
		WriteWithColor($"[{nameof(CsTreeTest)}]", ConsoleColor.Cyan);
		Println(null);
		Println($"Total={Summary.Total}");
		// 通過數非零才標綠，避免 0 通過時誤導性地顯示綠色。
		WriteWithColor(
			$"Passed={Summary.Passed}"
			,Summary.Passed > 0 ? ConsoleColor.Green : Console.ForegroundColor
		);
		Println(null);
		// 同上：有失敗才標紅。
		WriteWithColor(
			$"Failed={Summary.Failed}"
			,Summary.Failed > 0 ? ConsoleColor.Red : Console.ForegroundColor
		);
		Println(null);
		Println($"Elapsed={Summary.Elapsed}");
		Println(null);
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
		Println(Text);
		Console.ForegroundColor = OldColor;
	}
}
