using Tsinswreng.CsCore;

//using Tsinswreng.CsU128Id;

namespace Tsinswreng.CsTreeTest;

// 單檔：TestCase 純數據類型；無函數體可分離。
[Doc($$"""
{{nameof(ITestCase)}} 的默認實現：各字段都有安全默認值。
""")]
public class TestCase : ITestCase{
	[Doc($$"""
#See[{{nameof(ITestCase.FnTest)}}]
""")]
	public FnTest FnTest{get;set;} = null!;

	[Doc($$"""
#See[{{nameof(ITestCase.TesterType)}}]
""")]
	public Type? TesterType{get;set;}

	[Doc($$"""
#See[{{nameof(ITestCase.TesteeTypes)}}]
""")]
	public IList<Type> TesteeTypes{get;set;} = [];

	[Doc($$"""
#See[{{nameof(ITestCase.TesteeFnNames)}}]
""")]
	public IList<str> TesteeFnNames{get;set;} = [];

	[Doc($$"""
默認取 UUID。
#See[{{nameof(ITestCase.UniqName)}}]
""")]
	//public str UniqName{get;set;} = U128Id.NewUlid().ToLow64Base();
	public str UniqName{get;set;} = new Guid().ToString();
}
