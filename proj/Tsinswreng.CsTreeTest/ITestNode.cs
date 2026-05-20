using Tsinswreng.CsCore;
using Tsinswreng.CsU128Id;

namespace Tsinswreng.CsTreeTest;

[Doc(@$"Node of test tree.

Running a node of test tree  is to run all its children test cases.
")]
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


public class TestNode : ITestNode{
	public str UniqName{get;set;} = U128Id.NewUlid().ToLow64Base();
	public ITestCase? Data{get;set;}
	public IList<ITestNode> Children {get;set;} = [];
	public IList<ITestNode> Parents {get;set;} = [];
	public bool Ordered{get;set;} = false;
	public bool IsParallelRecursive{get;set;} = false;
}

public class TesterNode : TestNode, ITesterNode{
	public Type? TesterType{get;set;}
}

public static class ExtnITestNode{
	extension(ITestNode z){
		[Doc("Make a new child of self")]
		public ITestNode NewChild(str? UniqName = null){
			var R = new TestNode(){UniqName = UniqName ?? U128Id.NewUlid().ToLow64Base()};
			z.Children.Add(R);
			R.Parents.Add(z);
			return R;
		}
		public ITestNode NewChild<T>(str? UniqName = null){
			var R = new TesterNode(){
				UniqName = UniqName ?? U128Id.NewUlid().ToLow64Base()
				,TesterType = typeof(T)
			};
			z.Children.Add(R);
			R.Parents.Add(z);
			return R;
		}
	}
	extension(ITestNode z){
		[Doc(@$"Make a function to register `{nameof(FnTest)}`.
		this can help you simplify your code
		,without duplicately passing the same `{nameof(Type)}`
		")]
		[Obsolete(@$"use {nameof(MkTestFnRegister)}")]
		public Func<
			str, FnTest, nil
		> MkFnRegisterTest(
			Type TesterType
			,Type TesteeType
			,str UniqNamePrefix = ""
		){
			return (str UniqName, FnTest Fn) => {
				var Case = new TestCase{
					UniqName = UniqNamePrefix+UniqName,
					TesterType = TesterType,
					TesteeType = TesteeType,
					FnTest = Fn,
				};
				z.Children.Add(new TestNode{
					Data = Case,
				});
				return NIL;
			};
		}
		
		// [Doc(@$"Make a register to register `{nameof(FnTest)}`.
		// this can help you simplify your code
		// ,without duplicately passing the same `{nameof(Type)}`
		// ")]
		// [Obsolete]
		// public ITestFnRegister MkTestFnRegister(
		// 	Type TesterType
		// 	,Type TesteeType
		// 	,str UniqNamePrefix = ""
		// ){
		// 	return new TestFnRegister(z, TesterType, TesteeType, UniqNamePrefix);
		// }
		
		public ITestFnRegister MkTestFnRegister(
			Type TesterType
			,IList<Type> TesteeTypes
			,IList<str> TesteeFnNames
			,str UniqNamePrefix = ""
		){
			return new TestFnRegister(z, TesterType, TesteeTypes, TesteeFnNames, UniqNamePrefix);
		}
		
	}
}
