using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 分片聲明文件：本片測試 ExtnTestMgr.Walk；實現見 TestWalk.Impl.cs。
[Doc($$"""
#Sum[本檔只測 `ExtnTestMgr.Walk`：]
#Descr[
先序遍歷整棵子樹，
結果含自身。
]
""")]
public partial class TestMgrCase{
	[Doc($$"""
#Sum[把 `{{nameof(TestMgrExtn.Walk)}}` 的用例掛到節點上。]
#Params([[要掛用例的節點]])
""")]
	public partial void RegisterWalk(ITestNode Node);

	[Doc($$"""
#Sum[`{{nameof(TestMgrExtn.Walk)}}` 為先序遍歷且含自身：`A,B,D,C`。]
""")]
	public partial Task<nil> Walk_PreOrder_IncludesSelf(obj? O);
}