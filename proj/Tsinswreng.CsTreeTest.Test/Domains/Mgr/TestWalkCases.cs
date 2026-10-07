using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 分片聲明文件：本片測試 ExtnTestMgr.WalkCases；實現見 TestWalkCases.Impl.cs。
[Doc($$"""
#Sum[本檔只測 `ExtnTestMgr.WalkCases`：]
#Descr[
先序遍歷，
但只取帶用例的節點。
]
""")]
public partial class TestMgrCase{
	[Doc($$"""
#Sum[把 `{{nameof(ExtnTestMgr.WalkCases)}}` 的用例掛到節點上。]
#Params([[要掛用例的節點]])
""")]
	public partial void RegisterWalkCases(ITestNode Node);

	[Doc($$"""
#Sum[只取帶用例的節點：無用例的祖先與旁支都被跳過。]
""")]
	public partial Task<nil> WalkCases_OnlyCaseNodes_ReturnsLeafOnly(obj? O);
}