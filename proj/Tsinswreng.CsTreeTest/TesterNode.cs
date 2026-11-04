using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 單檔：TesterNode 純數據類型；無函數體可分離。
[Doc($$"""
{{nameof(TestNode)}} 之上多記測試類類型與實例的節點。
""")]
public class TesterNode : TestNode, ITesterNode{
	[Doc($$"""
#See[{{nameof(ITesterNode.TesterType)}}]
""")]
	public Type? TesterType{get;set;}

	[Doc($$"""
#See[{{nameof(ITesterNode.Tester)}}]
""")]
	public ITester? Tester{get;set;}
}
