using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：TestMgrExtn 擴展方法的宣告；實現見 TestMgrExtn.Impl.cs。
[Doc($$"""
遍歷、查找與篩選的擴展方法。

查找這幾個取不到就拋 `KeyNotFoundException`（面向「這東西必須存在」的場合）；
遍歷是惰性的，別多次消費同一個返回值。
""")]
public static partial class TestMgrExtn{
	[Doc($$"""
先序遍歷本節點及其全部子節點，含自身。不防環。
""")]
	public static partial IEnumerable<ITestNode> Walk(this ITestNode z);

	[Doc($$"""
先序遍歷，只取帶用例的節點（`{{nameof(ITestNode.Data)}}` 非空）。
""")]
	public static partial IEnumerable<ITestNode> WalkCases(this ITestNode z);

	[Doc($$"""
按測試類找節點；取不到拋 `KeyNotFoundException`。
""")]
	public static partial ITestNode GetNodeByTesterType(this ITestMgr z, Type TesterType);

	[Doc($$"""
按唯一名找節點；取不到拋 `KeyNotFoundException`。
沒顯式給名的節點名是 UUID，基本查不到。
""")]
	public static partial ITestNode GetNodeByName(this ITestMgr z, str UniqName);

	[Doc($$"""
按被測類型找節點；一條都沒有時拋例外。
""")]
	public static partial IList<ITestNode> GetNodesByTesteeType(this ITestMgr z, Type TesteeType);

	[Doc($$"""
按被測函數名找節點；函數名要用 `nameof` 賦值。
""")]
	public static partial IList<ITestNode> GetNodesByTesteeFnName(this ITestMgr z, str TesteeFnName);

	[Doc($$"""
開始一次篩選，範圍是整棵樹。

```cs
await Mgr.{{nameof(Filter)}}().{{nameof(TestFilter.TesteeFn)}}(nameof(ISvcUser.Login)).{{nameof(TestFilter.Select)}}().{{nameof(TreeTestExecutorExtn.RunEtPrint)}}();
```
""")]
	public static partial TestFilter Filter(this ITestMgr z);

	[Doc($$"""
在指定範圍內篩選；範圍外的用例即使命中條件也不會被選中。
#Prm[{{nameof(Scope)}}][範圍子樹的根]
""")]
	public static partial TestFilter Filter(this ITestMgr z, ITestNode Scope);
}
