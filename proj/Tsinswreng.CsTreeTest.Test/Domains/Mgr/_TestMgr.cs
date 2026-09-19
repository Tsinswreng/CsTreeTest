using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

/// 測測試管理器：
/// 分組、收編子管理器、初始化，
/// 以及按名/按類型/按被測函數查節點。
/// 本類是主檔：
/// 只負責把各被測函數的用例檔串起來，
/// 自己不寫用例。
public partial class TestMgrCase : ITester{
	public ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterInit(Node);
		RegisterAddBranch(Node);
		RegisterRegisterTesterIntoBranch(Node);
		RegisterRegisterSubMgr(Node);
		RegisterGetNodeByName(Node);
		RegisterGetNodeByTesterType(Node);
		RegisterGetNodesByTesteeType(Node);
		RegisterGetNodesByTesteeFnName(Node);
		RegisterWalk(Node);
		RegisterWalkCases(Node);
		return Node;
	}
}