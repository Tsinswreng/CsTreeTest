using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 分片聲明文件：本片測試 TestMgr.Init；實現見 TestInit.Impl.cs。
[Doc($$"""
#Sum[本檔只測 `TestMgr.Init`：]
#Descr[
重複初始化要拒絕，
重複註冊同一測試類也要在掛樹時被擋下。
]
""")]
public partial class TestMgrCase{
	[Doc($$"""
#Sum[把初始化防護的用例掛到節點上。]
#Params([[要掛用例的節點]])
""")]
	public partial void RegisterInit(ITestNode Node);

	[Doc($$"""
#Sum[管理器拒絕重複註冊同一測試類：同型節點掛樹時撞上重複檢查。]
""")]
	public partial Task<nil> Init_DupTesterType_Throws(obj? O);

	[Doc($$"""
#Sum[初始化前不可重複 `{{nameof(TestMgr.Init)}}`：第二次初始化必須被拒。]
""")]
	public partial Task<nil> Init_Twice_Throws(obj? O);
}