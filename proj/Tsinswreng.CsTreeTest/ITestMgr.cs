using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 單檔：ITestMgr 接口；純聲明，無函數可分離。
[Doc($$"""
#Sum[測試管理器：組裝測試樹、維護索引、提供查詢。]

#Descr[
職責只有三件：
組裝測試樹、
維護索引、
提供查詢。
它不跑用例（執行在 `{{nameof(TreeTestExecutor)}}`），
也不篩選（篩選在 `{{nameof(TestFilter)}}`）。

*它不管依賴從哪來。*
測試類的依賴由測試項目自己的全局依賴來源提供——
慣例是各測試項目自己寫一個靜態 `Di` 類，
測試類在自己的欄位初始化時用 `GetRSvc<T>()` 取。
因此本庫不引用、也不需要知道任何依賴注入相關的類型：
框架對依賴注入*零概念*。

*管理器自身不是測試類*，
故不實現 `{{nameof(ITester)}}`；
上層要收編下層時用 `{{nameof(TestMgr.RegisterSubMgr)}}`，
不要試圖把管理器當測試類註冊。
兩個測試類型的接口混用會讓測試樹上多出一層無用的「管理器節點」。

典型用法：
```cs
MyProj1TestMgr.Inst.Init();
var Hits = MyProj1TestMgr.Inst.Filter()
	.TesteeType<ISvcUser>()
	.TesteeFn(nameof(ISvcUser.Login))
	.Select();
await Hits.RunEtPrint();
```
]
""")]
public interface ITestMgr{
	[Doc($$"""
#Sum[測試樹的根。]
#Descr[
默認實現裏根的唯一名固定為 `Tests`，
使失敗輸出的路徑可讀。
篩選的默認範圍就是本節點。
]
""")]
	ITestNode Root{get;}

	[Doc($$"""
#Sum[索引。]
#Descr[
`{{nameof(TestMgr.Init)}}` 之後才包含全部節點；
`{{nameof(TestMgr.Init)}}` 之前只有根。
查詢與篩選一律走本索引，不重掃全樹。
]
""")]
	TestIndex Index{get;}

	[Doc($$"""
#Sum[收編另一個測試管理器的整棵子樹。]
#Descr[
只登記、不立刻掛樹：
真正的掛載與索引合併發生在本管理器的 `{{nameof(TestMgr.Init)}}` 裏。
這樣安排是因為本管理器的 `{{nameof(TestMgr.Init)}}` 會重建索引，
立刻併進來的條目會被那次重建清掉。

必須在本管理器 `{{nameof(TestMgr.Init)}}` *之前*調用；
`{{nameof(TestMgr.Init)}}` 之後再調用會直接拋例外。

下級管理器自身可以尚未 `{{nameof(TestMgr.Init)}}`，
`{{nameof(TestMgr.Init)}}` 時會代它初始化。
]
#Params([[要收編的下級管理器，不能為空；通常是各測試項目導出的單例]])
""")]
	void RegisterSubMgr(ITestMgr SubMgr);

	[Doc($$"""
#Sum[把已聲明的測試類實例化、掛到樹上，並建立索引；返回根節點。]
#Descr[
*只允許調用一次*，
第二次調用直接拋例外。

調用時機有硬性要求：
測試項目的全局依賴來源必須*先就緒*，
因為 `{{nameof(TestMgr.Init)}}` 會立刻 `new` 出測試類，
而測試類在欄位初始化時就取依賴。
順序顛倒會拿到空的依賴。

具體步驟見默認實現裏的註釋。
]
#Rtn[測試樹的根]
""")]
	ITestNode Init();
}
