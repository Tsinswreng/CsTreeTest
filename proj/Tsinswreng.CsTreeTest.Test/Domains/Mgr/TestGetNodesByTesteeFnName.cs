using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 分片聲明文件：本片測試 ExtnTestMgr.GetNodesByTesteeFnName；實現見 TestGetNodesByTesteeFnName.Impl.cs。
[Doc($$"""
#Sum[本檔只測 `ExtnTestMgr.GetNodesByTesteeFnName`：]
#Descr[
按被測函數名取相關用例節點。
]
""")]
public partial class TestMgrCase{
	[Doc($$"""
#Sum[把 `{{nameof(ExtnTestMgr.GetNodesByTesteeFnName)}}` 的用例掛到節點上。]
#Params([[要掛用例的節點]])
""")]
	public partial void RegisterGetNodesByTesteeFnName(ITestNode Node);

	[Doc($$"""
#Sum[按被測函數名取到的節點非空，且每條的 `{{nameof(ITestCase.TesteeFnNames)}}` 都含該函數名。]
""")]
	public partial Task<nil> GetNodesByTesteeFnName_FakeSvcEcho_AllNodesMatchTesteeFnName(obj? O);
}