using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 單檔：IResAddTestCase 接口；純聲明，無函數可分離。
[Doc($$"""
`{{nameof(ITestNodeExtn.AddTestCase)}}` 的結果。

目前只交出新建的用例節點；調用方要接着給這個節點設旗標、掛子節點時用它。
""")]
public interface IResAddTestCase{
	[Doc($$"""
	本條用例的節點，已掛在測試類的節點下。

	用例名即它自己的 `{{nameof(ITestNode.UniqName)}}`。
	""")]
	ITestNode NewTestNode{get;set;}
}
