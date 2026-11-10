using Tsinswreng.CsCore;

//using Tsinswreng.CsU128Id;

namespace Tsinswreng.CsTreeTest;

// 單檔：TestNode 純數據類型；無函數體可分離。
[Doc($$"""
{{nameof(ITestNode)}} 的默認實現：純數據。
""")]
public class TestNode : ITestNode{
	[Doc($$"""
默認取 UUID。
#See[{{nameof(ITestNode.UniqName)}}]
""")]
	public str UniqName{get;set;} = new Guid().ToString();

	[Doc($$"""
#See[{{nameof(ITestNode.Data)}}]
""")]
	public ITestCase? Data{get;set;}

	[Doc($$"""
#See[{{nameof(ITestNode.Children)}}]
""")]
	public IList<ITestNode> Children{get;set;} = [];

	[Doc($$"""
#See[{{nameof(ITestNode.Parents)}}]
""")]
	public IList<ITestNode> Parents{get;set;} = [];

	[Doc($$"""
默認為假：不約束直接子節點的先後，讓它們按階段並行，這是默認的最快路徑。
要串行就顯式設真。
#See[{{nameof(ITestNode.Ordered)}}]
""")]
	public bool Ordered{get;set;} = false;

	[Doc($$"""
默認為假：不禁並行，讓用例盡量同時跑，這是默認的最快路徑。
要讓整棵子樹獨佔執行就顯式設真。
#See[{{nameof(ITestNode.NonParallelizable)}}]
""")]
	public bool NonParallelizable{get;set;} = false;
}
