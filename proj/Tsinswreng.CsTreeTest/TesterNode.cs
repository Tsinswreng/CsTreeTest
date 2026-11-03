using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 單檔：TesterNode 純數據類型；無函數體可分離。
[Doc($$"""
#Sum[掛測試類的節點。]
#Descr[
在 `{{nameof(TestNode)}}` 之上多記一個測試類類型，
以及該測試類的實例。
]
""")]
public class TesterNode : TestNode, ITesterNode{
	[Doc(@$"#Sum[掛在本節點的測試類類型。]
#See[{nameof(ITesterNode.TesterType)}]")]
	public Type? TesterType{get;set;}

	[Doc(@$"#Sum[測試類實例，由測試管理器在 `{{nameof(TestMgr.Init)}}` 時填入。]
#See[{nameof(ITesterNode.Tester)}]")]
	public ITester? Tester{get;set;}
}
