using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 單檔：ITesterNode 接口；純聲明，無函數可分離。
[Doc($$"""
記着是哪個測試類註冊了自己的節點，由 `{{nameof(ITestNodeExtn.NewTesterChild)}}` 建立。
""")]
public interface ITesterNode:ITestNode{
	[Doc($$"""
註冊本節點的測試類類型；按測試類查節點與篩選都靠它。
""")]
	Type? TesterType{get;set;}

	[Doc($$"""
測試類實例。

由 `{{nameof(TestMgr.Init)}}` 自行 `new` 出來後填入——框架不經過依賴容器。
""")]
	ITester? Tester{get;set;}
}
