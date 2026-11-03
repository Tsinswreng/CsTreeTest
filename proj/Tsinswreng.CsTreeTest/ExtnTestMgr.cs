using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：ExtnTestMgr 擴展方法的宣告；實現見 ExtnTestMgr.Impl.cs。
[Doc($$"""
#Sum[測試樹的遍歷、查找與篩選入口的擴展方法。]
#Descr[
查找這幾個方法在取不到時*拋 `KeyNotFoundException`*，
與 `{{nameof(TestIndex)}}` 上那些「取不到返回空」的方法區分開：
擴展方法面向「這個東西必須存在」的場合，
取不到即為測試腳本寫錯。

遍歷這兩個方法是惰性的（`yield return`），
每次調用都重新走一遍樹，
不要多次消費同一個返回值。
]
""")]
public static partial class ExtnTestMgr{
	[Doc($$"""
#Sum[先序遍歷本節點及其全部遞迴子節點。]
#Descr[
含本節點自身。
次序即聲明次序，
篩選結果的穩定排序就靠這個次序。

*不防環*：
如果手工把節點接成環，本方法會無限遞迴；
按正常用法（`{{nameof(ExtnITestNode.NewChild)}}`、`{{nameof(ExtnITestNode.NewTesterChild)}}`）接出來的樹不會成環。
]
#Params([[遍歷起點，即 `this`]])
#Rtn[先序序列，含起點自身]
""")]
	public static partial IEnumerable<ITestNode> Walk(this ITestNode z);

	[Doc($$"""
#Sum[先序遍歷子樹，只取帶用例的節點。]
#Descr[
「帶用例」的判據是 `{{nameof(ITestNode.Data)}}` 非空；
分組節點與脫離原樹的臨時根都會被濾掉。
]
#Params([[遍歷起點，即 `this`]])
#Rtn[帶用例的節點序列，按先序]
""")]
	public static partial IEnumerable<ITestNode> WalkCases(this ITestNode z);

	[Doc($$"""
#Sum[按測試類找節點。]
#Descr[
「找不到就拋例外」的版本，
與 `{{nameof(TestIndex.FindByTesterType)}}` 的「找不到返回空」相對。
]
#Params([[測試管理器，即 `this`],
	[測試類類型]])
#Rtn[該測試類的節點]
""")]
	public static partial ITestNode GetNodeByTesterType(this ITestMgr z, Type TesterType);

	[Doc($$"""
#Sum[按唯一名找節點。]
#Descr[
節點名要可讀、且樹內唯一，才能真正按名定位；
沒顯式給名字的節點名是 ULID，基本查不到。
]
#Params([[測試管理器，即 `this`],
	[節點唯一名]])
#Rtn[該名字的節點]
""")]
	public static partial ITestNode GetNodeByName(this ITestMgr z, str UniqName);

	[Doc($$"""
#Sum[按被測類型找節點。]
#Descr[
一個被測類型可能對應多個節點（用例散在多個測試類下），
因此返回列表；
一條都沒有時拋例外。
]
#Params([[測試管理器，即 `this`],
	[被測類型]])
#Rtn[測該類型的用例節點]
""")]
	public static partial IList<ITestNode> GetNodesByTesteeType(this ITestMgr z, Type TesteeType);

	[Doc($$"""
#Sum[按被測函數名找節點。]
#Descr[
函數名要以 `nameof` 賦值，
手寫字符串改名後會靜默查不到，
在這裏表現為拋例外。
]
#Params([[測試管理器，即 `this`],
	[被測函數名]])
#Rtn[測該函數的用例節點]
""")]
	public static partial IList<ITestNode> GetNodesByTesteeFnName(this ITestMgr z, str TesteeFnName);

	[Doc($$"""
#Sum[開始一次篩選，範圍是整棵樹。]
#Descr[
篩選結果可直接執行：
```cs
await Mgr.Filter().TesteeFn(nameof(ISvcUser.Login)).Select().RunEtPrint();
```
]
#Params([[測試管理器，即 `this`]])
#Rtn[範圍為整棵樹的篩選器]
""")]
	public static partial TestFilter Filter(this ITestMgr z);

	[Doc($$"""
#Sum[在指定範圍內開始一次篩選。]
#Descr[
範圍之外的用例即使命中條件也不會被選中。
]
#Params([[測試管理器，即 `this`],
	[範圍子樹的根；傳根節點等於不限範圍]])
#Rtn[範圍為給定子樹的篩選器]
""")]
	public static partial TestFilter Filter(this ITestMgr z, ITestNode Scope);
}
