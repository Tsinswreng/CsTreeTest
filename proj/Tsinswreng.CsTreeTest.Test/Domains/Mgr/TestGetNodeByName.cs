using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 分片聲明文件：本片測試 ExtnTestMgr.GetNodeByName；實現見 TestGetNodeByName.Impl.cs。
[Doc($$"""
#Sum[本檔只測 `ExtnTestMgr.GetNodeByName`：]
#Descr[
按唯一名取節點，
取不到就拋 `KeyNotFound`。
]
""")]
public partial class TestMgrCase{
	[Doc($$"""
#Sum[把 `{{nameof(ExtnTestMgr.GetNodeByName)}}` 的用例掛到節點上。]
#Params([[要掛用例的節點]])
""")]
	public partial void RegisterGetNodeByName(ITestNode Node);

	[Doc($$"""
#Sum[按根節點的名字取到的就是 `{{nameof(ITestMgr.Root)}}` 本身。]
""")]
	public partial Task<nil> GetNodeByName_RootName_ReturnsRoot(obj? O);

	[Doc($$"""
#Sum[名字不存在時拋 `KeyNotFoundException`。]
""")]
	public partial Task<nil> GetNodeByName_UnknownName_ThrowsKeyNotFound(obj? O);
}