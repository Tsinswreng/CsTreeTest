using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

// 分片聲明文件：本片測試 ExtnTestMgr.Filter(範圍)；實現見 TestUnder.Impl.cs。
public partial class TestFilterCase{
	[Doc($$"""
#Sum[把按範圍篩選的用例掛到節點上。]
""")]
	public partial void RegisterUnder(ITestNode Node);

	[Doc($$"""
#Sum[Under 收窄範圍：給一個範圍節點，篩選只在那棵子樹內進行。]

#Descr[
每一條命中的用例，其路徑都應落在該子樹之下。
]
""")]
	public partial Task<nil> Under_NarrowScope_OnlySubtreeHits(obj? O);

	[Doc($$"""
#Sum[領域分組範圍下可按測試類再篩：限定範圍之後，再按測試類收窄。]
""")]
	public partial Task<nil> Under_DomainBranch_ThenTesterType_Exact(obj? O);
}