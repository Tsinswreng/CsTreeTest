using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Executor;

// 實現文件：只放函數實現。
public partial class TestExecutor{
	private static partial void AddCase(ITestNode Parent, str UniqName, FnTest Fn){
		Parent.NewChild(UniqName).Data = new TestCase{
			UniqName = UniqName,
			FnTest = Fn,
		};
	}

	private static partial async Task RunScratchTree(i32 Count, IList<i32> Seen, OptTestExecutor? Opt){
		var Host = new TestNode{UniqName = "Scratch"};
		for(var i = 0; i < Count; i++){
			var Captured = i;
			AddCase(Host, $"{i}", async (o) => {
				if(Captured % 2 == 0){
					await Task.Delay(20);
				}
				lock(Seen){
					Seen.Add(Captured);
				}
				return NIL;
			});
		}
		await new TreeTestExecutor().Run([Host], Opt ?? new OptTestExecutor{IsParallel = false});
	}

	public partial void RegisterRun(ITestNode Node){
		var R = Node.MkRegister<TestExecutor>(
			typeof(TreeTestExecutor)
			,nameof(TreeTestExecutor.Run)
			,"Order:"
		).Register;

		R(nameof(SequentialModeKeepsDeclaredOrder), SequentialModeKeepsDeclaredOrder!);
		R(nameof(ParallelModeRunsCasesConcurrently), ParallelModeRunsCasesConcurrently!);
		R(nameof(OptArgIsPassedToEachCase), OptArgIsPassedToEachCase!);
		R(nameof(FailedCaseIsRecordedAndDoesNotStopOthers), FailedCaseIsRecordedAndDoesNotStopOthers!);
		R(nameof(AllPassedSummaryIsCorrect), AllPassedSummaryIsCorrect!);
	}

	public partial async Task<nil> SequentialModeKeepsDeclaredOrder(obj? O){
		var T = Assert.IsTrue;
		var Seen = new List<i32>();
		await RunScratchTree(5, Seen, new OptTestExecutor{IsParallel = false});
		T(string.Join(",", Seen) == "0,1,2,3,4");
		return NIL;
	}

	public partial async Task<nil> ParallelModeRunsCasesConcurrently(obj? O){
		var T = Assert.IsTrue;
		var Probe = new ConcurrencyProbe();
		var Host = new TestNode{UniqName = "Par"};
		for(var i = 0; i < 4; i++){
			var Name = $"c{i}";
			AddCase(Host, Name, async (o2) => await Probe.Run());
		}
		var S = await new TreeTestExecutor().Run(
			[Host]
			,new OptTestExecutor{IsParallel = true, MaxDegreeOfParallelism = 4}
		);
		T(S.Total == 4 && S.Passed == 4);
		// 四條用例若真的並行，最大值必然大於 1。
		T(Probe.Max > 1);
		return NIL;
	}

	public partial async Task<nil> OptArgIsPassedToEachCase(obj? O){
		var T = Assert.IsTrue;
		var Host = new TestNode{UniqName = "ArgHost"};
		obj? Got = null;
		AddCase(Host, "c", async (A) => {
			Got = A;
			return NIL;
		});
		await new TreeTestExecutor().Run(
			[Host]
			,new OptTestExecutor{IsParallel = false, Arg = 42}
		);
		T(Got is int I && I == 42);
		return NIL;
	}

	public partial async Task<nil> FailedCaseIsRecordedAndDoesNotStopOthers(obj? O){
		var T = Assert.IsTrue;
		var Host = new TestNode{UniqName = "F"};
		AddCase(Host, "ok1", async (o2) => NIL);
		AddCase(Host, "bad", async (o2) => {
			throw new InvalidOperationException("故意的失敗");
		});
		AddCase(Host, "ok2", async (o2) => NIL);

		var S = await new TreeTestExecutor().Run(
			[Host]
			,new OptTestExecutor{IsParallel = false}
		);
		T(S.Total == 3);
		T(S.Passed == 2);
		T(S.Failed == 1);
		var Bad = S.Results.First(x => x.TestCase.UniqName == "bad");
		T(!Bad.IsPassed);
		T(Bad.Exception is not null);
		T(Bad.NodePath.EndsWith("bad"));
		return NIL;
	}

	public partial async Task<nil> AllPassedSummaryIsCorrect(obj? O){
		var T = Assert.IsTrue;
		var Host = new TestNode{UniqName = "P"};
		AddCase(Host, "a", async (o2) => NIL);
		AddCase(Host, "b", async (o2) => NIL);

		var S = await new TreeTestExecutor().Run(
			[Host]
			,new OptTestExecutor{IsParallel = false}
		);
		T(S.Total == 2 && S.Passed == 2 && S.Failed == 0);
		T(S.Results.All(x => x.IsPassed));
		return NIL;
	}
}