using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public partial class TestFnRegister{
	public partial TestFnRegister(
		ITestNode Node
		,Type TesterType
		,IList<Type> TesteeTypes
		,IList<str> TesteeFnNames
		,str UniqNamePrefix
	){
		this.Node = Node;
		this.TesterType = TesterType;
		this.TesteeTypes = TesteeTypes;
		this.TesteeFnNames = TesteeFnNames;
		this.UniqNamePrefix = UniqNamePrefix;
	}

	public partial void Register(str UniqName, FnTest Fn){
		var Case = new TestCase{
			UniqName = UniqNamePrefix + UniqName,
			TesterType = TesterType,
			TesteeTypes = TesteeTypes,
			TesteeFnNames = TesteeFnNames,
			FnTest = Fn,
		};
		this.Node.Children.Add(new TestNode{
			UniqName = Case.UniqName,
			Data = Case,
			Parents = [this.Node],
		});
	}
}

public static partial class ExtnITestFnRegister{
	public static partial ITestFnRegister MkRegister<TTester>(
		this ITestNode z
		,IList<Type>? TesteeTypes
		,IList<str>? TesteeFnNames
		,str UniqNamePrefix
	){
		return new TestFnRegister(
			z
			,typeof(TTester)
			,TesteeTypes ?? []
			,TesteeFnNames ?? []
			,UniqNamePrefix
		);
	}

	public static partial ITestFnRegister MkRegister(
		this ITestNode z
		,Type TesterType
		,IList<Type>? TesteeTypes
		,IList<str>? TesteeFnNames
		,str UniqNamePrefix
	){
		return new TestFnRegister(
			z
			,TesterType
			,TesteeTypes ?? []
			,TesteeFnNames ?? []
			,UniqNamePrefix
		);
	}

	public static partial ITestFnRegister MkRegister<TTester>(
		this ITestNode z
		,Type TesteeType
		,str TesteeFnName
		,str UniqNamePrefix
	){
		return new TestFnRegister(
			z
			,typeof(TTester)
			,[TesteeType]
			,[TesteeFnName]
			,UniqNamePrefix
		);
	}
}