using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：ITestNode 接口；只有成員聲明，無函數實現。
[Doc($$"""
測試樹節點。跑一個節點等於跑它底下的全部用例。

帶用例的節點是葉子（`{{nameof(Data)}}` 非空），只作分組的節點為空。
`{{nameof(Ordered)}}` 與 `{{nameof(IsParallelRecursive)}}` 互相獨立，別混用。
""")]
public interface ITestNode{
	[Doc($$"""
節點唯一名。同樹內不得重複，重複時建索引拋異常。

默認有 UUID 值，所以節點默認就能掛上去；
要按名查找（`{{nameof(TestMgrExtn.GetNodeByName)}}`）或要失敗路徑可讀時，才需要顯式給名。
""")]
	str UniqName{get;set;}

	[Doc($$"""
本節點攜帶的用例；分組節點為空。
""")]
	ITestCase? Data{get;set;}

	[Doc($$"""
子節點，順序即聲明順序。用 `{{nameof(ITestNodeExtn.NewChild)}}` 掛，別手工加。
""")]
	IList<ITestNode> Children{get;set;}

	[Doc($$"""
父節點。正常用法只有一個；`{{nameof(ITestNodeExtn.PathFromRoot)}}` 只取第一個。
""")]
	IList<ITestNode> Parents{get;set;}

	[Doc($$"""
為真時，直接子節點依序執行。不遞歸。

只管直接子節點之間的先後，子節點內部仍可並行。
要整棵子樹不與別的分支同時跑，用 `{{nameof(IsParallelRecursive)}}`。
""")]
	bool Ordered{get;set;}

	[Doc($$"""
為真時，本節點及其全部遞歸子節點都不參與並行。

向下繼承，標在最外層即可。默認為 `false`。
與 `{{nameof(Ordered)}}` 互相獨立，可同時生效。
""")]
	bool IsParallelRecursive{get;set;}
}
