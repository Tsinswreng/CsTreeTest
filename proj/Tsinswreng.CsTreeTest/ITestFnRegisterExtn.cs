using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：ITestFnRegisterExtn 擴展方法的宣告；實現見 ITestFnRegisterExtn.Impl.cs。
[Doc($$"""
建註冊器的擴展方法；三個重載按「知道多少元數據」區分。
""")]
public static partial class ITestFnRegisterExtn{
	[Doc($$"""
測試類類型取泛型參數；一個文件只測一個被測函數時最省事。

不給元數據時一個參數都別傳：寫 `{{nameof(MkRegister)}}<TestX>()`；
寫 `(null, null)` 會與下一個重載歧義（CS0121）。

#TPrm[{{nameof(TTester)}}][測試類類型，須實作 `{{nameof(ITester)}}`]
#Prm[{{nameof(UniqNamePrefix)}}][用例名前綴，默認不加]
#Rtn[註冊器；取它的 `{{nameof(ITestFnRegister.Register)}}` 方法組即可註冊用例]
""")]
	public static partial ITestFnRegister MkRegister<TTester>(
		this ITestNode z
		,IList<Type>? TesteeTypes = null
		,IList<str>? TesteeFnNames = null
		,str UniqNamePrefix = ""
	);

	[Doc($$"""
測試類類型也由調用方給，用於運行時才知道測試類的場合。

#Prm[{{nameof(TesterType)}}][測試類類型，不能為空]
#Rtn[註冊器]
""")]
	public static partial ITestFnRegister MkRegister(
		this ITestNode z
		,Type TesterType
		,IList<Type>? TesteeTypes = null
		,IList<str>? TesteeFnNames = null
		,str UniqNamePrefix = ""
	);

	[Doc($$"""
只用一個被測類型 + 一個被測函數名建註冊器。

#Prm[{{nameof(TesteeFnName)}}][必須用 `nameof`]
#Prm[{{nameof(UniqNamePrefix)}}][用例名前綴，默認不加]
#Rtn[註冊器]
""")]
	public static partial ITestFnRegister MkRegister<TTester>(
		this ITestNode z
		,Type TesteeType
		,str TesteeFnName
		,str UniqNamePrefix = ""
	);

	[Doc($$"""
	0.1.1 的舊名，與 `{{nameof(MkRegister)}}({{nameof(TesterType)}}, ...)` 完全等價。
	
	只為讓按舊版 API 寫的代碼照原樣編譯；新代碼請用 `{{nameof(MkRegister)}}`。
	
	#Prm[{{nameof(TesterType)}}][測試類類型，不能為空]
	#Prm[{{nameof(TesteeTypes)}}][被測類型列表]
	#Prm[{{nameof(TesteeFnNames)}}][被測函數名列表]
	#Prm[{{nameof(UniqNamePrefix)}}][用例名前綴，默認不加]
	#Rtn[註冊器]
	""")]
	public static partial ITestFnRegister MkTestFnRegister(
		this ITestNode z
		,Type TesterType
		,IList<Type> TesteeTypes
		,IList<str> TesteeFnNames
		,str UniqNamePrefix = ""
	);

	[Doc($$"""
	0.1.1 的舊名，與 `{{nameof(MkRegister)}}<{{nameof(TTester)}}>` 完全等價。
	
	只為讓按舊版 API 寫的代碼照原樣編譯；新代碼請用 `{{nameof(MkRegister)}}`。
	
	#TPrm[{{nameof(TTester)}}][測試類類型，須實作 `{{nameof(ITester)}}`]
	#Prm[{{nameof(TesteeTypes)}}][被測類型列表，可不給]
	#Prm[{{nameof(TesteeFnNames)}}][被測函數名列表，可不給]
	#Prm[{{nameof(UniqNamePrefix)}}][用例名前綴，默認不加]
	#Rtn[註冊器]
	""")]
	public static partial ITestFnRegister MkTestFnRegister<TTester>(
		this ITestNode z
		,IList<Type>? TesteeTypes = null
		,IList<str>? TesteeFnNames = null
		,str UniqNamePrefix = ""
	);
}
