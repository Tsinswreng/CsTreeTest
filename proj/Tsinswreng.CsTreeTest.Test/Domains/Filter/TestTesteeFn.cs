using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

// 分片聲明文件：本片測試 TestFilter.TesteeFn；實現見 TestTesteeFn.Impl.cs。
public partial class TestFilterCase{
	[Doc($$"""
#Sum[把 TesteeFn 的用例掛到節點上。]
""")]
	public partial void RegisterTesteeFn(ITestNode Node);

	[Doc($$"""
#Sum[按被測函數名篩選用例：命中的用例都測過該函數。]
""")]
	public partial Task<nil> TesteeFn_ByName_FiltersAllMatching(obj? O);
}