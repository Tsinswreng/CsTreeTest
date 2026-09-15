using Tsinswreng.CsTreeTest.Test.Domains.Assertions;
using Tsinswreng.CsTreeTest.Test.Domains.DiEtMgr;
using Tsinswreng.CsTreeTest.Test.Domains.Executor;
using Tsinswreng.CsTreeTest.Test.Domains.MkTestFnRegister;
using Tsinswreng.CsTreeTest.Test.Domains.NewChild;
using Tsinswreng.CsTreeTest.Test.Domains.TestFnRegister;

namespace Tsinswreng.CsTreeTest.Test;

// 命名不可含點號（模板坑），故用 CsTreeTestTestMgr 而非完整項目名。
public class CsTreeTestTestMgr:DiEtTestMgr{
	public static CsTreeTestTestMgr Inst = new();
	public override ITestNode RegisterTestsInto(ITestNode? Node){
		Node = this.TestNode;
		this.RegisterTester<TestTestNode>();
		this.RegisterTester<TestMkTestFnRegister>();
		this.RegisterTester<TestTestFnRegister>();
		this.RegisterTester<TestDiEtTestMgr>();
		this.RegisterTester<TestTreeTestExecutor>();
		this.RegisterTester<TestAssert>();
		return Node;
	}
}