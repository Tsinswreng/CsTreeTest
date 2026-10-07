using Tsinswreng.CsCore;
using Tsinswreng.CsU128Id;

namespace Tsinswreng.CsTreeTest;

[Doc($$"""
#Sum[單條用例。除了要跑的函數，還帶着它的元數據。]
#Descr[
元數據是查詢與篩選的唯一依據：
按測試類找、按被測類型找、按被測函數名找，
都只看這裏的字段。

元數據不是手寫的——
由 `{{nameof(ExtnITestFnRegister.MkRegister)}}` 在造註冊器時統一寫一遍，
之後每條用例只寫名字與函數體（見 `{{nameof(ITester)}}` 的示例）。
]
""")]
public interface ITestCase{
	[Doc($$"""
#Sum[用例函數本體。]
#Descr[
由註冊器寫入。
函數最後一個參數是框架傳進來的參數（`{{nameof(OptTestExecutor.Arg)}}`），
同步邏輯直接拋異常即算失敗。
]
""")]
	FnTest FnTest{get;set;}

	[Doc($$"""
#Sum[本用例屬於哪個測試類。]
#Descr[
由 `{{nameof(ExtnITestFnRegister.MkRegister)}}<TTester>` 的泛型參數決定，
一個用例只屬於一個測試類。
]
""")]
	Type? TesterType{get;set;}

	[Doc($$"""
#Sum[本用例測的是哪些被測類型。]
#Descr[
例：測 `ISvcUser.Login` 時此處為 `[typeof(ISvcUser)]`。
一個用例可以同時測多個被測類型。
]
""")]
	IList<Type> TesteeTypes{get;set;}

	[Doc($$"""
#Sum[本用例測的是哪些被測函數名。]
#Descr[
例：測 `ISvcUser.Login` 時此處為 `[nameof(ISvcUser.Login)]`。
*必須用 `nameof` 賦值*，
不要手寫字符串，
否則改名後篩選會靜默失效。
]
""")]
	IList<str> TesteeFnNames{get;set;}

	[Doc($$"""
#Sum[用例唯一名。]
#Descr[
同一棵測試樹內不得重複，
重複時建立索引會直接拋異常。
由註冊器按「用例名前綴 + 用例名」生成。
]
""")]
	str UniqName{get;set;}
}

// 用例默認實現：純數據類型，無方法體可分離（保持單文件）。
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
	public str UniqName{get;set;} = U128Id.NewUlid().ToLow64Base();
}