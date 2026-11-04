using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

// 分片聲明文件：本片測試 ExtnITestNode.PathString；實現見 TestPathString.Impl.cs。
[Doc($$"""
#Sum[本檔只測 `ExtnITestNode.PathString`：
把根到當前節點這一路的名字拼成字符串。]
#Descr[
*本檔只測這一個被測函數*，
新增別的被測函數的測試時請另開文件。
]
""")]
public partial class TestNodeTree{
	[Doc($$"""
#Sum[把 `{{nameof(ITestNodeExtn.PathString)}}` 的用例掛到節點上。]
#Params([[要掛用例的節點]])
""")]
	public partial void RegisterPathString(ITestNode Node);

	[Doc($$"""
#Sum[`{{nameof(ITestNodeExtn.PathString)}}` 把根到葉這一路的名字以 `/` 相連。]
""")]
	public partial Task<nil> PathString_NestedChain_RootToLeafSlashJoined(obj? O);
}