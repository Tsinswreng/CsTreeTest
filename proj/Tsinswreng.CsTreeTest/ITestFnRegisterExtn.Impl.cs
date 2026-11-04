using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public static partial class ITestFnRegisterExtn{
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

	// 舊名兼容：直接轉發，不另做一套，免得兩邊行為走樣。
	public static partial ITestFnRegister MkTestFnRegister(
		this ITestNode z
		,Type TesterType
		,IList<Type> TesteeTypes
		,IList<str> TesteeFnNames
		,str UniqNamePrefix
	){
		return z.MkRegister(TesterType, TesteeTypes, TesteeFnNames, UniqNamePrefix);
	}

	// 舊名兼容：直接轉發。
	public static partial ITestFnRegister MkTestFnRegister<TTester>(
		this ITestNode z
		,IList<Type>? TesteeTypes
		,IList<str>? TesteeFnNames
		,str UniqNamePrefix
	){
		return z.MkRegister<TTester>(TesteeTypes, TesteeFnNames, UniqNamePrefix);
	}
}
