using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：TestFnRegister 的成員宣告；實現見 TestFnRegister.Impl.cs。
[Doc($$"""
#Sum[註冊器的默認實現。]
#Descr[
一般不用直接 new；
用 `{{nameof(ITestFnRegisterExtn.MkRegister)}}` 擴展方法造。
]
""")]
public partial class TestFnRegister : ITestFnRegister{
	[Doc(@$"#Sum[用例要掛上去的那個節點。]")]
	private readonly ITestNode Node;

	[Doc(@$"#Sum[所屬測試類類型。]
#See[{nameof(ITestFnRegister.TesterType)}]")]
	public Type TesterType{get;set;}

	[Doc(@$"#Sum[被測類型列表。]
#See[{nameof(ITestFnRegister.TesteeTypes)}]")]
	public IList<Type> TesteeTypes{get;set;}

	[Doc(@$"#Sum[被測函數名列表。]
#See[{nameof(ITestFnRegister.TesteeFnNames)}]")]
	public IList<str> TesteeFnNames{get;set;}

	[Doc(@$"#Sum[用例名前綴。]
#See[{nameof(ITestFnRegister.UniqNamePrefix)}]")]
	public str UniqNamePrefix{get;set;}

	[Doc($$"""
	#Sum[造一個註冊器。]
	#Descr[各元數據直接寫入本對象，之後 `{{nameof(Register)}}` 時逐條快照給用例。]
	#Params([[用例要掛上去的節點],
		[所屬測試類類型],
		[被測類型列表],
		[被測函數名列表],
		[用例名前綴]])
	""")]
	public partial TestFnRegister(
		ITestNode Node
		,Type TesterType
		,IList<Type> TesteeTypes
		,IList<str> TesteeFnNames
		,str UniqNamePrefix
	);

	[Doc($$"""
	#Sum[註冊一條用例：按當前元數據造用例並掛成子節點。]
	#Descr[
	用例名是 `{{nameof(UniqNamePrefix)}} + {{nameof(UniqName)}}`，
	節點名與用例名相同。
	]
	#Params([[用例名，不含前綴],[用例函數本體]])
	""")]
	public partial void Register(str UniqName, FnTest Fn);
}
