using Tsinswreng.CsCore;
using Tsinswreng.CsU128Id;

namespace Tsinswreng.CsTreeTest;

// 樹節點。跑一個節點 = 跑它的所有子用例。
public interface ITestNode{
	
	public str UniqName{get;set;}
	
	[Doc(@$"Leaf node should hava {nameof(Data)}
	
	for non-leaf node, we suggest it should be null
	")]
	public ITestCase? Data{get;set;}
	public IList<ITestNode> Children {get;set;}
	
	[Doc(@$"Should only have one parent.
	
	but I design it as IList for be compatible with graph
	")]
	public IList<ITestNode> Parents {get;set;}
	[Doc(@$"If true, direct children(non recursive) should be run in order,
	{nameof(Ordered)} != Parallel. tests may be run in different thread.
	e.g when you need to insert test data first,
	then test db
	and finally remove test data.
	")]
	public bool Ordered{get;set;}
	[Doc(@$"default is false.
	if true, the node and its all recursive children should be
	run in the same thread
	")]
	public bool IsParallelRecursive{get;set;}
	
}

[Doc(@$"for {nameof(ITester)}")]
public interface ITesterNode:ITestNode{
	public Type? TesterType{get;set;}
}

// 樹節點默認實現：純數據類型，無方法體可分離（保持單文件）。
public class TestNode : ITestNode{
	public str UniqName{get;set;} = U128Id.NewUlid().ToLow64Base();
	public ITestCase? Data{get;set;}
	public IList<ITestNode> Children {get;set;} = [];
	public IList<ITestNode> Parents {get;set;} = [];
	public bool Ordered{get;set;} = false;
	public bool IsParallelRecursive{get;set;} = false;
}

/// 掛 tester 的節點：帶 TesterType。
public class TesterNode : TestNode, ITesterNode{
	public Type? TesterType{get;set;}
}