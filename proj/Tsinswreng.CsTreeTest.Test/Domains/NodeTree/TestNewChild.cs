using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

// 分片聲明文件：本片測試 ExtnITestNode.NewChild / NewTesterChild；實現見 TestNewChild.Impl.cs。
[Doc($$"""
#Sum[測建節點的各種途徑。]
#Descr[
*本檔主要測 `ExtnITestNode.NewChild`*，
並順帶測同族的 `{{nameof(ExtnITestNode.NewTesterChild)}}`（掛測試類節點的入口）。
兩者都會維護父子兩邊的引用。
]
""")]
public partial class TestNodeTree{
	[Doc($$"""
#Sum[把建節點的用例掛到節點上。]
#Params([[要掛用例的節點]])
""")]
	public partial void RegisterNewChild(ITestNode Node);

	[Doc($$"""
#Sum[不給名字時 `{{nameof(ExtnITestNode.NewChild)}}` 自動取非空默認名，並在父子兩邊都掛上引用。]
""")]
	public partial Task<nil> NewChild_NoName_DefaultUniqNameNonEmpty(obj? O);

	[Doc($$"""
#Sum[顯式給名字時 `{{nameof(ExtnITestNode.NewChild)}}` 用給的名字作唯一名。]
""")]
	public partial Task<nil> NewChild_ExplicitName_UniqNameAsGiven(obj? O);

	[Doc($$"""
#Sum[`{{nameof(ExtnITestNode.NewTesterChild)}}` 記下測試類類型，且不給名字時節點名取測試類短名。]
""")]
	public partial Task<nil> NewTesterChild_ExplicitTesterType_TypeRecordedAndNameIsClassName(obj? O);
}