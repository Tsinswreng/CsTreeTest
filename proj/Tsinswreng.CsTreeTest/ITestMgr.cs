using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 單檔：ITestMgr 接口；純聲明，無函數可分離。
[Doc($$"""
測試管理器：組裝測試樹、維護索引、提供查詢。

不跑用例（在 `{{nameof(TreeTestExecutor)}}`），也不篩選（在 `{{nameof(TestFilter)}}`）。
管理器自身不是測試類；合併多個測試工程用 `{{nameof(RegisterSubMgr)}}`，別把它當 tester 註冊。
""")]
public interface ITestMgr{
	[Doc($$"""
測試樹的根；默認實現的名固定為 `Tests`。
""")]
	ITestNode Root{get;}

	[Doc($$"""
索引；`{{nameof(Init)}}` 之後才包含全部節點。
""")]
	TestIndex Index{get;}

	[Doc($$"""
收編另一個管理器的整棵子樹。

只登記：掛樹與索引合併在 `{{nameof(Init)}}` 時做，`{{nameof(Init)}}` 之後再調用拋異常。
下級管理器可以尚未 `{{nameof(Init)}}`。

#Prm[{{nameof(SubMgr)}}][下級管理器，通常是各測試工程的單例]
""")]
	void RegisterSubMgr(ITestMgr SubMgr);

	[Doc($$"""
實例化已聲明的測試類、掛樹、建索引，返回根。只允許調用一次。

全局依賴來源必須先就緒：`{{nameof(Init)}}` 會立刻 `new` 出測試類，而它們在欄位初始化時就取依賴。

#Rtn[測試樹的根]
""")]
	ITestNode Init();
}
