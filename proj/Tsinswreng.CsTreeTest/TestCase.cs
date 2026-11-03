using Tsinswreng.CsCore;

//using Tsinswreng.CsU128Id;

namespace Tsinswreng.CsTreeTest;

// 單檔：TestCase 純數據類型；無函數體可分離。
[Doc($$"""
#Sum[用例的默認實現。]
#Descr[
各字段都有安全默認值：
唯一名默認取 ULID，
兩個元數據列表默認為空列表。
]
""")]
public class TestCase : ITestCase{
	[Doc(@$"#Sum[用例函數本體；由註冊器寫入。]
#See[{nameof(ITestCase.FnTest)}]")]
	public FnTest FnTest{get;set;} = null!;

	[Doc(@$"#Sum[所屬測試類類型；由註冊器寫入。]
#See[{nameof(ITestCase.TesterType)}]")]
	public Type? TesterType{get;set;}

	[Doc(@$"#Sum[被測類型列表，默認空。]
#See[{nameof(ITestCase.TesteeTypes)}]")]
	public IList<Type> TesteeTypes{get;set;} = [];

	[Doc(@$"#Sum[被測函數名列表，默認空。]
#See[{nameof(ITestCase.TesteeFnNames)}]")]
	public IList<str> TesteeFnNames{get;set;} = [];

	[Doc(@$"#Sum[唯一名，默認取 ULID。]
#See[{nameof(ITestCase.UniqName)}]")]
	//public str UniqName{get;set;} = U128Id.NewUlid().ToLow64Base();
	public str UniqName{get;set;} = new Guid().ToString();
}
