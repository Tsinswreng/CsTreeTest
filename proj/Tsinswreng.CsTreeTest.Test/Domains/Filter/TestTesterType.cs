using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

// 分片聲明文件：本片測試 TestFilter.TesterType；實現見 TestTesterType.Impl.cs。
public partial class TestFilterCase{
	[Doc($$"""
#Sum[把 TesterType 的用例掛到節點上。]
""")]
	public partial void RegisterTesterType(ITestNode Node);

	[Doc($$"""
#Sum[按測試類篩選用例：命中的用例都屬於該測試類。]
""")]
	public partial Task<nil> TesterType_ByTesterType_FiltersAllMatching(obj? O);
}