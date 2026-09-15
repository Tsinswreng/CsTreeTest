using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Executor;

public partial class TestTreeTestExecutor{
	public partial void RegisterOrderAndArg(ITestNode Node){
		var R = Node.MkTestFnRegister<TestTreeTestExecutor>().Register;
		var T = Assert.IsTrue;

		R("Ordered 節點按聲明順序執行", async (o)=>{
			var Log = new List<str>();
			var Root = new TestNode{ Ordered = true };
			Root.Children.Add(MkLeaf("First", async o2 => { Log.Add("first"); await Task.Delay(30); return null; }));
			Root.Children.Add(MkLeaf("Second", async o2 => { Log.Add("second"); return null; }));

			await new TreeTestExecutor().RunTests(Root, Opt: new OptTestExecutor{ IsParallel = false });
			T(Log.SequenceEqual(["first", "second"]));
			return null;
		});

		R("Arg 傳遞給每個用例", async (o)=>{
			var Root = new TestNode();
			Root.Children.Add(MkLeaf("ArgCase", async o2 => {
				Assert.IsTrue(o2 as string == "hello", $"arg was: {o2}");
				return null;
			}));

			var S = await new TreeTestExecutor().RunTests(Root, "hello", new OptTestExecutor{ IsParallel = false });
			T(S.Failed == 0);
			return null;
		});
	}
}