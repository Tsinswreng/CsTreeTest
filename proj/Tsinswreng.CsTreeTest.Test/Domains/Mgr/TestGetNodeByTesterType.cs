using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 分片聲明文件：本片測試 ExtnTestMgr.GetNodeByTesterType；實現見 TestGetNodeByTesterType.Impl.cs。
[Doc($$"""
#Sum[本檔只測 `ExtnTestMgr.GetNodeByTesterType`：]
#Descr[
按測試類類型取它掛的那個節點。
]
""")]
public partial class TestMgrCase{
	[Doc($$"""
#Sum[把 `{{nameof(TestMgrExtn.GetNodeByTesterType)}}` 的用例掛到節點上。]
#Params([[要掛用例的節點]])
""")]
	public partial void RegisterGetNodeByTesterType(ITestNode Node);

	[Doc($$"""
#Sum[按測試類類型取到的節點，其 `{{nameof(ITesterNode.TesterType)}}` 即該類型。]
""")]
	public partial Task<nil> GetNodeByTesterType_RegisteredTester_ReturnsItsNode(obj? O);
}