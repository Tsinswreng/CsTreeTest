using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

// 分片聲明文件：本片測試 TestFilter.Where；實現見 TestWhere.Impl.cs。
public partial class TestFilterCase{
	[Doc($$"""
#Sum[把 Where 的用例掛到節點上。]
""")]
	public partial void RegisterWhere(ITestNode Node);

	[Doc($$"""
#Sum[Where 自訂條件：用自訂謂詞按用例名篩選。]

#Descr[
謂詞取 Executor 域那條用例的方法名，
命中的用例全部滿足該謂詞。
]
""")]
	public partial Task<nil> Where_CustomPredicate_CaseNameFragment_Filters(obj? O);
}