using Microsoft.Extensions.DependencyInjection;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.DiEtMgr;

public partial class TestDiEtTestMgr{
	public partial void RegisterQueryTestCase(ITestNode Node){
		var R = Node.MkTestFnRegister<TestDiEtTestMgr>().Register;
		var T = Assert.IsTrue;

		R("按被測類型查到的節點 Data 是該用例 TestCase", async (o)=>{
			var Mgr = new LocalDiMgr();
			Mgr.RegisterTester<DummyTester>();
			Mgr.InitSvc(new ServiceCollection(), sc => sc.BuildServiceProvider());

			var ByType = Mgr.GetNodesByTesteeType(typeof(IFakeSvc));
			T(ByType.Count == 1);
			var TC = ByType[0].Data as TestCase;
			T(TC is not null);
			T(TC!.UniqName == "D:dummy case");                       // prefix + 用例名
			T(TC.TesterType == typeof(DummyTester));
			T(TC.TesteeTypes.SequenceEqual([typeof(IFakeSvc)]));
			T(TC.TesteeFnNames.SequenceEqual([nameof(IFakeSvc.Foo)]));
			return null;
		});

		R("按被測函數名查到的也是同一用例", async (o)=>{
			var Mgr = new LocalDiMgr();
			Mgr.RegisterTester<DummyTester>();
			Mgr.InitSvc(new ServiceCollection(), sc => sc.BuildServiceProvider());

			var ByFn = Mgr.GetNodesByTesteeFnName(nameof(IFakeSvc.Foo));
			T(ByFn.Count == 1);
			T(ByFn[0].Data is TestCase FNCase && FNCase.UniqName == "D:dummy case");
			return null;
		});

		R("tester 節點下可枚舉出用例（樹形查詢）", async (o)=>{
			var Mgr = new LocalDiMgr();
			Mgr.RegisterTester<DummyTester>(NewNodeUniqName: "T1");
			Mgr.InitSvc(new ServiceCollection(), sc => sc.BuildServiceProvider());

			var TesterNode = Mgr.GetNodeByName("T1");
			var TC = TesterNode.Children[0].Data as TestCase;
			T(TC is not null && TC!.UniqName == "D:dummy case");
			return null;
		});

		R("tester 節點本身 Data 為空（不是用例）", async (o)=>{
			var Mgr = new LocalDiMgr();
			Mgr.RegisterTester<DummyTester>(NewNodeUniqName: "T2");
			Mgr.InitSvc(new ServiceCollection(), sc => sc.BuildServiceProvider());

			var TesterNode = Mgr.GetNodeByTesterType(typeof(DummyTester));
			T(TesterNode.Data is null);
			return null;
		});
	}
}