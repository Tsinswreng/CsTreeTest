using Microsoft.Extensions.DependencyInjection;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.DiEtMgr;

public partial class TestDiEtTestMgr{
	public partial void RegisterIndexAndLookup(ITestNode Node){
		var R = Node.MkTestFnRegister<TestDiEtTestMgr>().Register;
		var T = Assert.IsTrue;

		R("RegisterTester 建立節點並可按名查找", async (o)=>{
			var Mgr = new LocalDiMgr();
			Mgr.RegisterTester<DummyTester>(NewNodeUniqName: "KnownName");
			Mgr.InitSvc(new ServiceCollection(), sc => sc.BuildServiceProvider());
			var N = Mgr.GetNodeByName("KnownName");
			T(N is ITesterNode TN && TN.TesterType == typeof(DummyTester));
			return null;
		});

		R("按 TesterType 查找節點", async (o)=>{
			var Mgr = new LocalDiMgr();
			Mgr.RegisterTester<DummyTester>();
			Mgr.InitSvc(new ServiceCollection(), sc => sc.BuildServiceProvider());
			var N = Mgr.GetNodeByTesterType(typeof(DummyTester));
			T(N is ITesterNode TN && TN.TesterType == typeof(DummyTester));
			return null;
		});

		R("按被測類型與被測函數名查找", async (o)=>{
			var Mgr = new LocalDiMgr();
			Mgr.RegisterTester<DummyTester>();
			Mgr.InitSvc(new ServiceCollection(), sc => sc.BuildServiceProvider());
			var ByType = Mgr.GetNodesByTesteeType(typeof(IFakeSvc));
			var ByFn = Mgr.GetNodesByTesteeFnName(nameof(IFakeSvc.Foo));
			T(ByType.Count >= 1);
			T(ByFn.Count >= 1);
			T(ByType[0] == ByFn[0]);
			return null;
		});
	}
}