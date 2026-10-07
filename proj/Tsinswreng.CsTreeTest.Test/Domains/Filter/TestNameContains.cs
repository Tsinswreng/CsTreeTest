using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

// 分片聲明文件：本片測試 TestFilter.NameContains；實現見 TestNameContains.Impl.cs。
public partial class TestFilterCase{
	[Doc($$"""
#Sum[把 NameContains 的用例掛到節點上。]
""")]
	public partial void RegisterNameContains(ITestNode Node);

	[Doc($$"""
#Sum[按名稱片段篩選：片段只命中一條用例。]

#Descr[
片段取 Executor 域那條用例的方法名，
期望挑到的唯一名為「前綴 + 用例名」。
]
""")]
	public partial Task<nil> NameContains_CaseNameFragment_ExactSingleHit(obj? O);

	[Doc($$"""
#Sum[無命中時返回空列表。]
""")]
	public partial Task<nil> NameContains_NoMatch_ReturnsEmpty(obj? O);
}