using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：TestFnRegister 的成員宣告；實現見 TestFnRegister.Impl.cs。
[Doc($$"""
註冊器的默認實現；用 `{{nameof(ITestFnRegisterExtn.MkRegister)}}` 造，別直接 new。
""")]
public partial class TestFnRegister : ITestFnRegister{
	[Doc($$"""
用例要掛上去的那個節點。
""")]
	private readonly ITestNode Node;

	[Doc($$"""
#See[{{nameof(ITestFnRegister.TesterType)}}]
""")]
	public Type TesterType{get;set;}

	[Doc($$"""
#See[{{nameof(ITestFnRegister.TesteeTypes)}}]
""")]
	public IList<Type> TesteeTypes{get;set;}

	[Doc($$"""
#See[{{nameof(ITestFnRegister.TesteeFnNames)}}]
""")]
	public IList<str> TesteeFnNames{get;set;}

	[Doc($$"""
#See[{{nameof(ITestFnRegister.UniqNamePrefix)}}]
""")]
	public str UniqNamePrefix{get;set;}

	[Doc($$"""
造一個註冊器，各元數據原樣記下。

#Prm[{{nameof(Node)}}][用例要掛上去的節點]
""")]
	public partial TestFnRegister(
		ITestNode Node
		,Type TesterType
		,IList<Type> TesteeTypes
		,IList<str> TesteeFnNames
		,str UniqNamePrefix
	);

	[Doc($$"""
按當前元數據造用例並掛成子節點，節點名與用例名相同。
#See[{{nameof(ITestFnRegister.Register)}}]
""")]
	public partial void Register(str UniqName, FnTest Fn);
}
