using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：TestIndex 的成員宣告；實現見 TestIndex.Impl.cs。
[Doc($$"""
測試樹節點的四組索引，查詢與篩選的唯一依據。

前兩組一對一：撞名、或同一測試類註冊兩次（且非同一節點實例）時拋 `InvalidOperationException`。
後兩組一對多，只登記帶用例的葉節點。
""")]
public partial class TestIndex{
	[Doc($$"""
唯一名到節點，一對一；唯一名為空的不進本組。
""")]
	public IDictionary<str, ITestNode> UniqName_TestNode{get;set;}
		= new Dictionary<str, ITestNode>();

	[Doc($$"""
測試類類型到節點，一對一；不屬於任何測試類的節點不進本組。
""")]
	public IDictionary<Type, ITestNode> TesterType_TestNode{get;set;}
		= new Dictionary<Type, ITestNode>();

	[Doc($$"""
被測類型到節點，一對多；列表內不重複記同一節點。
""")]
	public IDictionary<Type, IList<ITestNode>> TesteeType_TestNode{get;set;}
		= new Dictionary<Type, IList<ITestNode>>();

	[Doc($$"""
被測函數名到節點，一對多。鍵是函數名不是簽名，不同類型的同名方法會撞在一起。
""")]
	public IDictionary<str, IList<ITestNode>> TesteeFnName_TestNode{get;set;}
		= new Dictionary<str, IList<ITestNode>>();

	[Doc($$"""
把節點連同它的用例元數據記進索引，測試類類型取自節點自身。

#Prm[{{nameof(Node)}}][非 `{{nameof(ITesterNode)}}` 的節點只登記唯一名]
""")]
	public partial void Add(ITestNode Node);

	[Doc($$"""
同上，測試類類型由參數給。

重複登記同一節點實例是允許的；被測函數名為空白的條目會被跳過。

#Prm[{{nameof(TesterType)}}][為空表示不屬於任何測試類，只登記唯一名]
""")]
	public partial void Add(ITestNode Node, Type? TesterType);

	[Doc($$"""
把另一個索引的條目併入本索引；收編下級管理器時用。

一對多的兩組按追加合併，測試類那組直接覆蓋。

#Prm[{{nameof(SkipUniqName)}}][併唯一名這組時跳過的鍵，一般是下級管理器的根：已不在樹上且與父根同名]
""")]
	public partial void MergeFrom(TestIndex Other, str? SkipUniqName = null);

	[Doc($$"""
按唯一名取節點；取不到返回空。
""")]
	public partial ITestNode? FindByName(str UniqName);

	[Doc($$"""
按測試類取節點；取不到（沒註冊過）返回空。
""")]
	public partial ITestNode? FindByTesterType(Type TesterType);

	[Doc($$"""
按被測類型取節點；取不到返回空列表。
取到時返回的是索引內的列表本身，別在外面增刪。
""")]
	public partial IList<ITestNode> FindByTesteeType(Type TesteeType);

	[Doc($$"""
按被測函數名取節點；取不到返回空列表。同上，別改返回的列表。
""")]
	public partial IList<ITestNode> FindByTesteeFnName(str TesteeFnName);

	[Doc($$"""
往一對多字典追加節點；鍵不存在先建空列表，按引用去重。
""")]
	private static partial void AddToDict<TKey>(
		IDictionary<TKey, IList<ITestNode>> Dict
		,TKey Key
		,ITestNode Node
	) where TKey:notnull;
}
