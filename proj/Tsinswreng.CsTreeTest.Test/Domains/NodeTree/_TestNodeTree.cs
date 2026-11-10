using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.NodeTree;

// 聲明文件：只寫函數聲明（partial method）與 Doc；實現見 _TestNodeTree.Impl.cs。
[Doc($$"""
#Sum[測樹節點自身的形狀。]
#Descr[
本類是主檔：
只負責把各被測函數的用例檔串起來，
自己不寫用例。
分片有 `TestNewChild.cs`、`TestPathString.cs`、`TestPathFromRoot.cs`。
]
""")]
public partial class TestNodeTree : ITester{
	[Doc($$"""
#Sum[把本測試類的全部用例掛到給定節點上。]
#Descr[
本方法由測試樹在 `Init()` 時調用，
`Node` 一定非空（就是掛本測試類的那個節點）。
`Node ??= new TestNode()` 是爲了讓本測試類也能脫離測試樹單獨跑。

本領域的用例各建各的樹，互不干擾，
所以不設 `{{nameof(ITestNode.Ordered)}}`、`{{nameof(ITestNode.NonParallelizable)}}` 這兩個開關。
]
#Params([[框架給的掛載點；測試樹一定傳非空值]])
#Rtn[掛好用例的那個節點]
""")]
	public partial ITestNode RegisterTestsInto(ITestNode? Node);
}