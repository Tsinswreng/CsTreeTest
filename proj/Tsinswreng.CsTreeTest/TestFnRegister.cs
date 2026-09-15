using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 註冊器：把用例註冊到節點。每次 Register 用最新 option 字段。
public interface ITestFnRegister{
	[Doc(@$" every time you call `{nameof(Register)}`, it should always use newest options.
	which means you can call `{nameof(Register)}` after you change other option fields
	")]
	public void Register(str UniqName, FnTest Fn);
	
	/// 以下字段皆可讀寫
	public Type TesterType{get;set;}
	public IList<Type> TesteeTypes{get;set;}
	public IList<str> TesteeFnNames{get;set;}
	public str UniqNamePrefix{get;set;}
	
}

// 聲明文件：字段/屬性 + 構造函數/方法聲明（partial method）；實現見 TestFnRegister.Impl.cs。
public partial class TestFnRegister : ITestFnRegister{
	private readonly ITestNode Node;
	public Type TesterType{get;set;}
	public IList<Type> TesteeTypes{get;set;}
	public IList<str> TesteeFnNames{get;set;}
	public str UniqNamePrefix{get;set;}

	/// 舊形態：單一 testee 類型。
	public partial TestFnRegister(ITestNode node, Type testerType, Type testeeType, str uniqNamePrefix);
	
	/// 完整形態：testee 類型與函數名列表一次傳入。
	public partial TestFnRegister(
		ITestNode node
		,Type testerType
		,IList<Type> testeeTypes
		,IList<str> testeeFnNames
		,str uniqNamePrefix
	);

	/// 註冊一個用例到節點；UniqName = prefix + 傳入名。
	public partial void Register(str UniqName, FnTest Fn);
}