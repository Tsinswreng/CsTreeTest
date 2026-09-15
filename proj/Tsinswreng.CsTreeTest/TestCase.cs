using Tsinswreng.CsCore;
using Tsinswreng.CsU128Id;

namespace Tsinswreng.CsTreeTest;

// 用例：一個用例只測一個函數（也可以把多個函數併成一個用例測）。
public interface ITestCase{
	[Doc(@$"Function to test")]
	public FnTest FnTest{get;set;}
	
	[Doc(@$"Clr type that contains the {nameof(FnTest)},
	that is the tester class.
	
	one {nameof(ITestCase)} can only belong to one {nameof(ITester)}
	
	this may be used to filter in the future
	")]
	public Type? TesterType{get;set;}
	
	[Obsolete(@$"Use {nameof(TesteeTypes)} instead")]
	public Type? TesteeType{
		get{
			return TesteeTypes.FirstOrDefault();
		}
		set{
			if(value is not null){
				TesteeTypes.Add(value);
			}
		}
	}
	

	[Doc(@$"Clr types of testee. you can test more than one type in one case.
	
	e.g if you test `ISvcUser.Login()` then this should be `[typeof(ISvcUser)]`
	
	this may be used to filter in the future
	")]
	public IList<Type> TesteeTypes{get;set;}
	
	[Doc(@$"function names of testee. you can test more than one function in one case.
	
	e.g if you test `ISvcUser.Login()` then this should be `[nameof(ISvcUser.Login)]`.
	Use `nameof` to assign.
	
	this may be used to filter in the future
	")]
	public IList<str> TesteeFnNames{get;set;}
	
	public str UniqName{get;set;}
	
}

// 用例的默認實現：純數據類型，無方法體可分離（保持單文件）。UniqName 默認取 ULID。
public class TestCase : ITestCase{
	public FnTest FnTest{get;set;}
	public Type? TesterType{get;set;}
	public str UniqName{get;set;} = U128Id.NewUlid().ToLow64Base();
	public IList<Type> TesteeTypes{get;set;} = [];
	public IList<str> TesteeFnNames{get;set;} = [];
	[Obsolete(@$"Use {nameof(TesteeTypes)} instead")]
	public Type? TesteeType{
		get{
			return TesteeTypes.FirstOrDefault();
		}
		set{
			if(value is not null){
				TesteeTypes.Add(value);
			}
		}
	}
	
}