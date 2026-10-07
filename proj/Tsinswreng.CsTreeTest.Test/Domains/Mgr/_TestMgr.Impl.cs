using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 實現文件：只放函數實現。
public partial class TestMgrCase{
	public partial ITestNode RegisterTestsInto(ITestNode? Node){
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