using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 分片聲明文件：本片測試 TestMgr.AddBranch；實現見 TestAddBranch.Impl.cs。
[Doc($$"""
#Sum[本檔只測 `TestMgr.AddBranch`：]
#Descr[
在根下建領域分組節點，
同名時復用同一個。
]
""")]
public partial class TestMgrCase{
	[Doc($$"""
#Sum[把 `{{nameof(TestMgr.AddBranch)}}` 的用例掛到節點上。]
#Params([[要掛用例的節點]])
""")]
	public partial void RegisterAddBranch(ITestNode Node);

	[Doc($$"""
#Sum[同名 `{{nameof(TestMgr.AddBranch)}}` 建出的分組必須是同一節點，否則唯一名會撞。]
""")]
	public partial Task<nil> AddBranch_SameNameTwice_ReusesSameNode(obj? O);
}