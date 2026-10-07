using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

// 分片聲明文件：本片測試 TestFilter.Apply；實現見 TestApply.Impl.cs。
public partial class TestFilterCase{
	[Doc($$"""
#Sum[把 Apply 的用例掛到節點上。]
""")]
	public partial void RegisterApply(ITestNode Node);

	[Doc($$"""
#Sum[多個條件之間是「同時滿足」：既要測 IFakeSvc、又要是 TestRegister 這個測試類的。]

#Descr[
與被測項無交集的測試類（TestNodeTree）交叉後應為空。
]
""")]
	public partial Task<nil> Apply_TesterTypeAndTesteeType_AndIntersect_Exact(obj? O);
}