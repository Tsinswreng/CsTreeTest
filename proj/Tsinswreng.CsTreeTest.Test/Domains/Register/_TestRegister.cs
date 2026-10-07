using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Register;

// 聲明文件：只寫函數聲明（partial method）與 Doc；實現見 _TestRegister.Impl.cs。
[Doc($$"""
#Sum[測註冊器（MkRegister / TestFnRegister）。]
#Descr[
本類同時演示「測試類從全局依賴來源取被測對象」：
欄位初始化時直接 `GetRSvc`，
不需要建構子參數。

本類是主檔：
只負責把各被測函數的用例檔串起來，
自己不寫用例。
分片有 `TestMkRegister.cs`。
]
""")]
public partial class TestRegister : ITester{
	[Doc($$"""
#Sum[被測對象。]
#Descr[
欄位初始化時就向全局依賴來源取，
因此*全局依賴來源必須在測試管理器 `{{nameof(TestMgr.Init)}}()` 之前就緒*。
]
""")]
	private readonly IFakeSvc _Svc = GetRSvc<IFakeSvc>();

	[Doc($$"""
#Sum[把本測試類的全部用例掛到給定節點上。]
#Descr[
本方法由測試管理器在 `{{nameof(TestMgr.Init)}}()` 時調用，
`Node` 一定非空（就是掛本測試類的那個節點）。
`Node ??= new TestNode()` 是爲了讓本測試類也能脫離測試樹單獨跑。

本領域的用例各建各的節點，互不干擾，
所以不設 `{{nameof(ITestNode.Ordered)}}`、`{{nameof(ITestNode.IsParallelRecursive)}}` 這兩個開關。
]
#Params([[框架給的掛載點；測試管理器一定傳非空值]])
#Rtn[掛好用例的那個節點]
""")]
	public partial ITestNode RegisterTestsInto(ITestNode? Node);
}