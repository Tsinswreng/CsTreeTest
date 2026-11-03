using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：TestIndex 的成員宣告；實現見 TestIndex.Impl.cs。
[Doc($$"""
#Sum[測試索引：把測試樹上的節點按四種鍵各記一份，供查詢與篩選使用。]

#Descr[
*四組索引各自怎麼建、各有甚麼約束*：

+ `{{nameof(TestIndex.UniqName_TestNode)}}`（唯一名到節點，一對一）：
	節點名必須在同一棵樹內唯一；
	同名但*不是同一個節點實例*時，
	登記直接拋例外。
+ `{{nameof(TestIndex.TesterType_TestNode)}}`（測試類類型到節點，一對一）：
	一個測試類只註冊一個節點；
	同一個測試類被登記兩次（兩個不同實例）也拋例外。
+ `{{nameof(TestIndex.TesteeType_TestNode)}}`（被測類型到節點，一對多）：
	一個被測類的用例可能散在多個測試類下，
	所以值是列表。
+ `{{nameof(TestIndex.TesteeFnName_TestNode)}}`（被測函數名到節點，一對多）：
	理由同上。

*索引是查詢與篩選的唯一依據。*
樹上每掛一次東西（測試類、領域分組、下級管理器）之後都要同步更新索引，
否則查詢結果會與樹不一致：
查得到卻不在樹上、或樹上有卻查不到。

只有*帶用例的葉節點*會進被測類型與被測函數名這兩組索引；
分組節點的 `{{nameof(ITestNode.Data)}}` 為空，
因而不會進來。
]
""")]
public partial class TestIndex{
	[Doc($$"""
#Sum[唯一名到節點，一對一。]
#Descr[
每個鍵只對應一個節點。
唯一名為空的節點不進本組。
重複的鍵（不同節點實例）由 `Add` 攔下並拋例外。
]
""")]
	public IDictionary<str, ITestNode> UniqName_TestNode{get;set;}
		= new Dictionary<str, ITestNode>();

	[Doc($$"""
#Sum[測試類類型到節點，一對一。]
#Descr[
每個測試類只註冊一個節點。
沒有測試類歸屬的節點（如領域分組、子管理器的掛載節點）不進本組。
]
""")]
	public IDictionary<Type, ITestNode> TesterType_TestNode{get;set;}
		= new Dictionary<Type, ITestNode>();

	[Doc($$"""
#Sum[被測類型到節點，一對多。]
#Descr[
一對多：
一個被測類的用例可能散在多個測試類下。
列表內不重複記同一個節點。
]
""")]
	public IDictionary<Type, IList<ITestNode>> TesteeType_TestNode{get;set;}
		= new Dictionary<Type, IList<ITestNode>>();

	[Doc($$"""
#Sum[被測函數名到節點，一對多。]
#Descr[
一對多。
鍵是函數名（不是方法簽名），
因此*不同類型的同名方法會撞在一起*，
這是按函數名篩選時已知的粒度限制。
]
""")]
	public IDictionary<str, IList<ITestNode>> TesteeFnName_TestNode{get;set;}
		= new Dictionary<str, IList<ITestNode>>();

	[Doc($$"""
#Sum[把一個節點連同它的用例元數據記進索引，測試類類型由節點自身取。]
#Descr[
節點實現了 `{{nameof(ITesterNode)}}` 時取其 `TesterType`，
否則測試類類型記為空——
即*測試類這組不登記*，
唯一名與用例元數據兩組照常登記。
]
#Params([[要入索引的節點]])
""")]
	public partial void Add(ITestNode Node);

	[Doc($$"""
#Sum[把一個節點連同它的用例元數據記進索引，測試類類型由參數顯式給。]
#Descr[
唯一名或測試類的鍵已存在、
且對應的不是*同一個節點實例*時拋 `InvalidOperationException`：
前者意味着兩個節點撞名，
後者意味着同一個測試類被註冊了兩次。

重複登記*同一個*節點實例是允許的（覆蓋自己），
子樹入索引時就會走到這條路。

被測函數名為空白的條目會被跳過（記了也查不到）。
]
#Params([[要入索引的節點],
	[測試類類型；為空表示這個節點不屬於任何測試類，只登記唯一名]])
""")]
	public partial void Add(ITestNode Node, Type? TesterType);

	[Doc($$"""
#Sum[把另一個索引的全部條目併入本索引。]
#Descr[
收編下級測試管理器時用，
否則上層按測試類或按被測函數名查不到下級的用例。

`SkipUniqName` 用來跳過「已經不在樹上」的那個節點條目，
一般是下級管理器自己的根：
它已不在掛好之後的樹上，
而且默認名也是 `Tests`，
併進來會與父根互相覆蓋。

*為甚麼是「跳過」而不是「先併入再刪」*：
本索引裏可能已經有同名的合法條目（父根就叫 `Tests`），
先併入再刪會把那個合法條目一起誤刪；
索引也沒有 `Remove` 這類接口，
所以「跳過」是唯一安全的路子。

一對多的兩組按*追加*合併，
因此不會覆蓋已有的合法條目；
測試類那組是一對一、直接覆蓋。
]
#Params([[要併入的索引，不能為空],
	[合併唯一名這組時要跳過的鍵；為空時不跳過]])
""")]
	public partial void MergeFrom(TestIndex Other, str? SkipUniqName = null);

	[Doc($$"""
#Sum[按唯一名取節點。]
#Descr[
取不到返回空，
由調用方決定是拋例外還是當作沒有——
`{{nameof(ExtnTestMgr.GetNodeByName)}}` 選的是拋例外。
]
#Params([[節點唯一名]])
#Rtn[節點；查不到時為空]
""")]
	public partial ITestNode? FindByName(str UniqName);

	[Doc($$"""
#Sum[按測試類取節點。]
#Descr[
只適用於已登記的測試類；
該測試類沒註冊過時返回空。
]
#Params([[測試類類型]])
#Rtn[測試類節點；查不到時為空]
""")]
	public partial ITestNode? FindByTesterType(Type TesterType);

	[Doc($$"""
#Sum[按被測類型取節點。]
#Descr[
取不到時返回*新建的空列表*而不是 `null`，
調用方可直接遍歷。

取到時返回的是*索引內部的列表本身，不是副本*，
請勿在外部增刪它，
否則索引會與樹不一致。
]
#Params([[被測類型]])
#Rtn[測該類型的用例節點；查不到時為空列表]
""")]
	public partial IList<ITestNode> FindByTesteeType(Type TesteeType);

	[Doc($$"""
#Sum[按被測函數名取節點。]
#Descr[
函數名應以 `nameof` 賦值，
手寫字符串在改名後會靜默查不到。

與 `{{nameof(TestIndex.FindByTesteeType)}}` 一樣：
取不到返回空列表，
取到時返回索引內部的列表本身，請勿外部修改。
]
#Params([[被測函數名]])
#Rtn[測該函數的用例節點；查不到時為空列表]
""")]
	public partial IList<ITestNode> FindByTesteeFnName(str TesteeFnName);

	[Doc($$"""
#Sum[往一對多的字典裏追加節點，同一個節點不重複加。]
#Descr[
鍵不存在時先建空列表，
這樣調用方不必先判斷鍵在不在。

去重靠 `List.Contains`；
`{{nameof(ITestNode)}}` 未重寫相等比較，
因此比的是*引用*——
同一個節點被多條路徑帶進來時只會記一次。
]
#TParams([鍵的類型，不能為空])
#Params([[目標字典],
	[鍵],
	[要追加的節點]])
""")]
	private static partial void AddToDict<TKey>(
		IDictionary<TKey, IList<ITestNode>> Dict
		,TKey Key
		,ITestNode Node
	) where TKey:notnull;
}
