using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 單檔：ITesterNode 接口；純聲明，無函數可分離。
[Doc($$"""
#Sum[掛測試類的節點：樹節點之外，還記着是哪個測試類註冊了它。]
#Descr[
由 `{{nameof(ITestNodeExtn.NewTesterChild)}}` 建立。
一般不用直接 new 本接口的實現類。
]
""")]
public interface ITesterNode:ITestNode{
	[Doc($$"""
#Sum[註冊本節點的測試類類型。]
#Descr[
由 `{{nameof(ITestNodeExtn.NewTesterChild)}}` 寫入。
索引與篩選（按測試類找節點）就是靠它。
]
""")]
	Type? TesterType{get;set;}

	[Doc($$"""
#Sum[測試類實例。]
#Descr[
框架不經過依賴容器，
測試類由測試管理器在 `{{nameof(TestMgr.Init)}}` 時自行 `new` 出來並記在此處，
以便需要時能從樹反查到測試類對象。
]
""")]
	ITester? Tester{get;set;}
}
