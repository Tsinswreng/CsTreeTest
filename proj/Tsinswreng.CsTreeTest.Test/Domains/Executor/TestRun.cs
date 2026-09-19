using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Executor;

/// 本檔只測 TreeTestExecutor.Run：
/// 依序與並行兩種模式、
/// 參數傳遞、
/// 失敗記錄、
/// 結果匯總。
public partial class TestExecutor{
	/// 往節點下掛一條用例，
	/// 用例體由 `Fn` 給。
	private static void AddCase(ITestNode Parent, str UniqName, FnTest Fn){
		Parent.NewChild(UniqName).Data = new TestCase{
			UniqName = UniqName,
			FnTest = Fn,
		};
	}

	/// 建一棵首層帶 `Count` 個用例的臨時樹並執行。
	/// 每個用例把執行的次序記進 `Seen`，
	/// 用例之間以奇偶交錯的延時放大「亂序」的風險：
	/// 若執行器真的把次序弄亂，這裡會露餡。
	private static async Task RunScratchTree(i32 Count, IList<i32> Seen, OptTestExecutor? Opt){
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

	/// 把 Run 的用例掛到節點上。
	public void RegisterRun(ITestNode Node){
		var R = Node.MkRegister<TestExecutor>(
			typeof(TreeTestExecutor)
			,nameof(TreeTestExecutor.Run)
			,"Order:"
		).Register;
		var T = Assert.IsTrue;

		R("依序模式保持聲明次序", async (o) => {
			var Seen = new List<i32>();
			await RunScratchTree(5, Seen, new OptTestExecutor{IsParallel = false});
			T(string.Join(",", Seen) == "0,1,2,3,4");
			return NIL;
		});

		R("並行模式確實同時跑多條用例", async (o) => {
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
		});

		R("Opt.Arg 傳給每條用例", async (o) => {
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
		});

		R("失敗用例被記下且不中斷其餘用例", async (o) => {
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
		});

		R("全部通過時統計正確", async (o) => {
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
		});
	}
}