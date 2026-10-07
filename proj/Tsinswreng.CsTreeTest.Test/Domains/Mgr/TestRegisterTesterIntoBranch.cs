using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 分片聲明文件：本片測試 TestMgr.RegisterTesterIntoBranch；實現見 TestRegisterTesterIntoBranch.Impl.cs。
[Doc($$"""
#Sum[本檔只測 `TestMgr.RegisterTesterIntoBranch`：]
#Descr[
把測試類掛到指定領域分組之下，
而不是直接掛在根下。
]
""")]
public partial class TestMgrCase{
	[Doc($$"""
#Sum[把 `{{nameof(TestMgr.RegisterTesterIntoBranch)}}` 的用例掛到節點上。]
#Params([[要掛用例的節點]])
""")]
	public partial void RegisterRegisterTesterIntoBranch(ITestNode Node);

	[Doc($$"""
#Sum[測試類節點的父級是分組節點而非根。]
""")]
	public partial Task<nil> RegisterTesterIntoBranch_TesterNode_MountedUnderBranch(obj? O);
}