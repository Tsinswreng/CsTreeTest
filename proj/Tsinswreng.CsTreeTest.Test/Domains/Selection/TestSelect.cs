using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

namespace Tsinswreng.CsTreeTest.Test.Domains.Selection;

// 分片聲明文件：本片測試 TestSelection.Select；實現見 TestSelect.Impl.cs。
[Doc($$"""
#Sum[本檔只測 TestSelection.Select：]
#Descr[
由篩選器造出選集，
選集數量與用例清單要和條件一致。
]
""")]
public partial class TestSelectionCase{
	[Doc($$"""
#Sum[把 Select 的用例掛到節點上。]
""")]
	public partial void RegisterSelect(ITestNode Node);

	[Doc($$"""
#Sum[Select 得到選集且可數。]
""")]
	public partial Task<nil> SelectYieldsCountableSelection(obj? O);
}