using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Executor;

// 實現文件：只放函數實現。
public partial class TestExecutor{
	public partial void RegisterOrdered(ITestNode Node){
		var R = Node.MkRegister<TestExecutor>(
			typeof(ITestNode)
			,nameof(ITestNode.Ordered)
			,"Ordered:"
		).Register;

		R(nameof(OrderedAffectsOnlyDirectChildrenAndIsNotRecursive), OrderedAffectsOnlyDirectChildrenAndIsNotRecursive!);
	}

	public partial async Task<nil> OrderedAffectsOnlyDirectChildrenAndIsNotRecursive(obj? O){
		var T = Assert.IsTrue;
		var Probe = new ConcurrencyProbe();
		var Finished = new List<i32>();
		var Host = new TestNode{UniqName = "Ord", Ordered = true};
		for(var b = 0; b < 3; b++){
			var Branch = Host.NewChild($"B{b}");
			for(var i = 0; i < 2; i++){
				var Captured = b * 10 + i;
				AddCase(Branch, $"{i}", async (o2) => {
					await Probe.Run(30);
					lock(Finished){
						Finished.Add(Captured);
					}
					return NIL;
				});
			}
		}
		var S = await new TreeTestExecutor().Run(
			[Host]
			,new OptTestExecutor{IsParallel = true, MaxDegreeOfParallelism = 4}
		);
		T(S.Total == 6 && S.Passed == 6);
		// 直接子節點依序：
		// 同一分支的兩條用例都跑完，下一分支才開始。
		T(Finished.Count == 6);
		for(var b = 0; b < 3; b++){
			T(Finished[b * 2] / 10 == b);
			T(Finished[b * 2 + 1] / 10 == b);
		}
		// 不遞歸：
		// 單個分支內部的兩條用例仍然可以同時跑。
		T(Probe.Max > 1);
		return NIL;
	}
}