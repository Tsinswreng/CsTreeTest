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
#See[{{nameof(ITestNode.Ordered)}}]
""")]
	public bool Ordered{get;set;} = false;

	[Doc($$"""
#See[{{nameof(ITestNode.IsParallelRecursive)}}]
""")]
	public bool IsParallelRecursive{get;set;} = false;
}
