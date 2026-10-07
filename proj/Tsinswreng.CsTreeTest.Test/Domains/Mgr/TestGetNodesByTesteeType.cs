using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 分片聲明文件：本片測試 ExtnTestMgr.GetNodesByTesteeType；實現見 TestGetNodesByTesteeType.Impl.cs。
[Doc($$"""
#Sum[本檔只測 `ExtnTestMgr.GetNodesByTesteeType`：]
#Descr[
按被測類型取相關用例節點。
]
""")]
public partial class TestMgrCase{
	[Doc($$"""
#Sum[把 `{{nameof(ExtnTestMgr.GetNodesByTesteeType)}}` 的用例掛到節點上。]
#Params([[要掛用例的節點]])
""")]
	public partial void RegisterGetNodesByTesteeType(ITestNode Node);

	[Doc($$"""
#Sum[按被測類型取到的節點非空，且每條的 `{{nameof(ITestCase.TesteeTypes)}}` 都含該類型。]
""")]
	public partial Task<nil> GetNodesByTesteeType_FakeSvc_AllNodesMatchTesteeType(obj? O);
}