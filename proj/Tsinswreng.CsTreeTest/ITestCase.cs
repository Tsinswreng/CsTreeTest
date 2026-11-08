using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 單檔：ITestCase 接口；純聲明，無函數可分離。
[Doc($$"""
單條用例：要跑的函數 + 查詢與篩選用的元數據。

元數據由 `{{nameof(ITestFnRegisterExtn.MkRegister)}}` 統一寫入，不必手寫。
""")]
public interface ITestCase{
	[Doc($$"""
用例本體。正常返回即通過，拋出任何異常即失敗。
""")]
	FnTest FnTest{get;set;}

	[Doc($$"""
所屬測試類，由 `{{nameof(ITestFnRegisterExtn.MkRegister)}}` 的泛型參數決定。
""")]
	Type? TesterType{get;set;}

	[Doc($$"""
本用例測的被測類型。例：測 `ISvcUser.Login` 時為 `[typeof(ISvcUser)]`。
""")]
	IList<Type> TesteeTypes{get;set;}

	[Doc($$"""
本用例測的被測函數名。必須用 `nameof` 賦值，手寫字符串改名後會靜默失效。
""")]
	IList<str> TesteeFnNames{get;set;}

	[Obsolete($$"""
	{{nameof(ITestCase)}}掛在{{nameof(ITestNode)}}芝潙葉節者。
	而TestNode已經有UniqName
	""")]
	str UniqName{get;set;}
}
