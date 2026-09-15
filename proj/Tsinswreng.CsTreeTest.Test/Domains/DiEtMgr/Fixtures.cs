using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.DiEtMgr;

/// 假被測接口：只為用例 metadata（TesteeTypes/TesteeFnNames）提供索引目標。
public interface IFakeSvc{ void Foo(); }

/// 假 tester：無參構造即可由 DI 解析，註冊一個帶 metadata 的用例。
/// 接口+實現類模式，完整定義（不拆分）；RegisterTestsInto 是本類對接口的實現。
public class DummyTester:ITester{
	public ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		var R = Node.MkTestFnRegister<DummyTester>(
			[typeof(IFakeSvc)], [nameof(IFakeSvc.Foo)], "D:"
		).Register;
		R("dummy case", async (o)=>{ return null; });
		return Node;
	}
}

/// 本地 DiEtTestMgr：每個用例各自 new 實例，避免跨用例共享狀態。
public class LocalDiMgr:DiEtTestMgr{
	public override ITestNode RegisterTestsInto(ITestNode? Node){
		Node = this.TestNode;
		return Node;
	}
}