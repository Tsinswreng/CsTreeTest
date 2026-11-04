using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：ITestNode 接口；只有成員聲明，無函數實現。
[Doc($$"""
#Sum[測試樹節點。跑一個節點等於跑它底下的全部用例。]

#Descr[
樹由節點組成。
帶用例的節點是葉子（`{{nameof(Data)}}` 非空），
只作分組的節點不帶用例（`{{nameof(Data)}}` 為空）。

節點有兩個控制執行方式的開關，
它們互相獨立，
不要混用：

+ `{{nameof(Ordered)}}`：
	只管*直接子節點*之間的先後次序，
	不遞歸。
	適合「先插測試數據、再測、最後清理」這種有先後依賴的場景。
+ `{{nameof(IsParallelRecursive)}}`：
	為真時*本節點及其全部遞歸子節點*都不參與並行，
	即只與兄弟依序輪流，
	不與別的分支同時跑。
	本聲明向下繼承。
	適合「本子樹必須獨佔某個共享資源」的場景。
]
""")]
public interface ITestNode{
	[Doc($$"""
#Sum[節點唯一名。]
#Descr[
同一棵測試樹內不得重複，
重複時建立索引會直接拋異常。

有UUID默認值
所以節點默認就能掛上去；
需要按名查找（`{{nameof(ExtnTestMgr.GetNodeByName)}}`）、
或希望失敗輸出的路徑可讀時，
應顯式指定可讀的名字。
]
""")]
	str UniqName{get;set;}

	[Doc($$"""
#Sum[該節點攜帶的用例。]
#Descr[
葉節點帶用例；
非葉節點只作分組，此處為空。
]
""")]
	ITestCase? Data{get;set;}

	[Doc($$"""
#Sum[子節點，順序即聲明順序。]
#Descr[
用 `{{nameof(ExtnITestNode.NewChild)}}` 掛子節點時會自動維護父子兩邊的引用，
不要手工往這個列表裏加。
]
""")]
	IList<ITestNode> Children{get;set;}

	[Doc($$"""
#Sum[父節點。]
#Descr[
正常用法下一個節點只有一個父節點；
聲明為列表是為了保留圖結構的可能。
向前追溯到根的 `{{nameof(ExtnITestNode.PathFromRoot)}}` 只會取第一個父節點。
]
""")]
	IList<ITestNode> Parents{get;set;}

	[Doc($$"""
#Sum[為真時，該節點的直接子節點依序執行。]
#Descr[
*不遞歸*：
只約束該節點的直接子節點之間的次序，
再往下的節點不受本聲明影響——
單個子節點內部仍然可以並行。

`{{nameof(Ordered)}}` 不等於「不並行」。
要讓整棵子樹都不與外部同時跑，
用 `{{nameof(IsParallelRecursive)}}`。

例：某節點有三個子節點「插數據、測、清數據」，
把它設為 `true` 即可保證這三步的先後；
這三個子節點各自內部仍然是並行的。
]
""")]
	bool Ordered{get;set;}

	[Doc($$"""
#Sum[為真時，該節點及其全部遞歸子節點不參與並行。]
#Descr[
即：
本子樹只與自己的兄弟依序輪流，
不與別的分支同時跑。
本聲明*向下繼承*，
標在最外層即可約束整棵子樹。

默認為 `false`。
與 `{{nameof(Ordered)}}` 的區別：
`{{nameof(Ordered)}}` 管的是「直接子節點之間的次序」，
本開關管的是「本子樹能不能與別的分支同時跑」，
兩者可以同時使用。
]
""")]
	bool IsParallelRecursive{get;set;}
}
