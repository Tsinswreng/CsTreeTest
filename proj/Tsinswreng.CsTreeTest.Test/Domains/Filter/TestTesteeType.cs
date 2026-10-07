using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

// 分片聲明文件：本片測試 TestFilter.TesteeType；實現見 TestTesteeType.Impl.cs。
public partial class TestFilterCase{
	[Doc($$"""
#Sum[把 TesteeType 的用例掛到節點上。]
""")]
	public partial void RegisterTesteeType(ITestNode Node);

	[Doc($$"""
#Sum[按被測類型篩選用例：命中的用例都測過該類型。]
""")]
	public partial Task<nil> TesteeType_ByType_FiltersAllMatching(obj? O);
}