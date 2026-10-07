using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

namespace Tsinswreng.CsTreeTest.Test.Domains.Selection;

// 分片聲明文件：本片測試 TestSelection.MkDetachedTree；實現見 TestMkDetachedTree.Impl.cs。
[Doc($$"""
#Sum[本檔只測 TestSelection.MkDetachedTree：]
#Descr[
把選中的用例掛成一棵獨立的扁平臨時樹，
次序與篩選結果一致。
]
""")]
public partial class TestSelectionCase{
	[Doc($$"""
#Sum[把 MkDetachedTree 的用例掛到節點上。]
""")]
	public partial void RegisterMkDetachedTree(ITestNode Node);

	[Doc($$"""
#Sum[選集的臨時樹保持原次序。]
""")]
	public partial Task<nil> DetachedTreeKeepsOriginalOrder(obj? O);
}