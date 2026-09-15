using Microsoft.Extensions.DependencyInjection;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.DiEtMgr;

public partial class TestDiEtTestMgr{
	public partial void RegisterSubMgr(ITestNode Node){
		var R = Node.MkTestFnRegister<TestDiEtTestMgr>().Register;
		var T = Assert.IsTrue;

		R("RegisterSubMgr 把子 Mgr 的測試掛載到父樹", async (o)=>{
			var Sub = new LocalDiMgr();
			Sub.RegisterTester<DummyTester>(NewNodeUniqName: "SubTester");
			var Parent = new LocalDiMgr();
			Parent.RegisterSubMgr(Sub);
			Parent.InitSvc(new ServiceCollection(), sc => sc.BuildServiceProvider());

			T(Sub.TestNode.Children.Count == 1);
			T(Sub.TestNode.Children[0] is ITesterNode TN && TN.TesterType == typeof(DummyTester));
			return null;
		});
	}
}