using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

// 分片聲明文件：本片測試 ExtnITestNode.PathFromRoot；實現見 TestPathFromRoot.Impl.cs。
[Doc($$"""
#Sum[本檔只測 `ExtnITestNode.PathFromRoot`：
返回從根到當前節點的名字列表。]
#Descr[
*本檔只測這一個被測函數*，
新增別的被測函數的測試時請另開文件。
]
""")]
public partial class TestNodeTree{
	[Doc($$"""
#Sum[把 `{{nameof(ITestNodeExtn.PathFromRoot)}}` 的用例掛到節點上。]
#Params([[要掛用例的節點]])
""")]
	public partial void RegisterPathFromRoot(ITestNode Node);

	[Doc($$"""
#Sum[`{{nameof(ITestNodeExtn.PathFromRoot)}}` 返回從根到本節點這一路各節點的唯一名，含兩端。]
""")]
	public partial Task<nil> PathFromRoot_NestedChain_RootToLeafNames(obj? O);
}