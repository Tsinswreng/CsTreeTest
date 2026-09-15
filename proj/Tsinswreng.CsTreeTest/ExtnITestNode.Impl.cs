using Tsinswreng.CsCore;
using Tsinswreng.CsU128Id;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public static partial class ExtnITestNode{
	public static partial ITestNode NewChild(this ITestNode z, str? UniqName){
		var R = new TestNode(){UniqName = UniqName ?? U128Id.NewUlid().ToLow64Base()};
		z.Children.Add(R);
		R.Parents.Add(z);
		return R;
	}

	public static partial ITestNode NewChild<T>(this ITestNode z, str? UniqName){
		var R = new TesterNode(){
			UniqName = UniqName ?? U128Id.NewUlid().ToLow64Base()
			,TesterType = typeof(T)
		};
		z.Children.Add(R);
		R.Parents.Add(z);
		return R;
	}

	public static partial Func<str, FnTest, nil> MkFnRegisterTest(
		this ITestNode z
		,Type TesterType
		,Type TesteeType
		,str UniqNamePrefix
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

	public static partial ITestFnRegister MkTestFnRegister(
		this ITestNode z
		,Type TesterType
		,IList<Type> TesteeTypes
		,IList<str> TesteeFnNames
		,str UniqNamePrefix
	){
		return new TestFnRegister(z, TesterType, TesteeTypes, TesteeFnNames, UniqNamePrefix);
	}

	public static partial ITestFnRegister MkTestFnRegister<TTester>(
		this ITestNode z
		,IList<Type>? TesteeTypes
		,IList<str>? TesteeFnNames
		,str UniqNamePrefix
	){
		return new TestFnRegister(
			z, typeof(TTester)
			,TesteeTypes ?? []
			,TesteeFnNames ?? []
			,UniqNamePrefix
		);
	}
}