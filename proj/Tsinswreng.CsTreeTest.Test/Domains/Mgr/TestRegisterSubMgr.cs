using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 分片聲明文件：本片測試 TestMgr.RegisterSubMgr；實現見 TestRegisterSubMgr.Impl.cs。
[Doc($$"""
#Sum[本檔只測 `TestMgr.RegisterSubMgr`：]
#Descr[
把下級管理器的整棵子樹收編上來，
並把它的索引併進本管理器的索引。
]
""")]
public partial class TestMgrCase{
	[Doc($$"""
#Sum[把 `{{nameof(TestMgr.RegisterSubMgr)}}` 的用例掛到節點上。]
#Params([[要掛用例的節點]])
""")]
	public partial void RegisterRegisterSubMgr(ITestNode Node);

	[Doc($$"""
#Sum[收編子管理器後上層能按被測函數名、被測類型找到下級用例，且子樹按名可尋。]
""")]
	public partial Task<nil> RegisterSubMgr_SubMgr_MergesIndex(obj? O);

	[Doc($$"""
#Sum[收編時下級根節點的 `{{nameof(ITestNode.Ordered)}}`/`{{nameof(ITestNode.IsParallelRecursive)}}` 設定要原樣帶上。]
""")]
	public partial Task<nil> RegisterSubMgr_SubMgrRootOrdered_PreservesOrderCfg(obj? O);
}