using Microsoft.Extensions.DependencyInjection;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.DiEtMgr;

public partial class TestDiEtTestMgr{
	public partial void RegisterDuplicateGuard(ITestNode Node){
		var R = Node.MkTestFnRegister<TestDiEtTestMgr>().Register;
		var T = Assert.IsTrue;

		R("重複註冊同 tester 類型拋 InvalidOperationException", async (o)=>{
			var Mgr = new LocalDiMgr();
			Mgr.RegisterTester<DummyTester>(NewNodeUniqName: "A");
			Mgr.RegisterTester<DummyTester>(NewNodeUniqName: "B");
			try{
				Mgr.InitSvc(new ServiceCollection(), sc => sc.BuildServiceProvider());
				T(false);
			}catch(InvalidOperationException){
				// 預期的重複保護
			}
			return null;
		});
	}
}