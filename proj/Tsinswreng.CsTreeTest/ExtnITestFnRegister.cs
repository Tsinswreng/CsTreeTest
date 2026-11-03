using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：ExtnITestFnRegister 擴展方法的宣告；實現見 ExtnITestFnRegister.Impl.cs。
[Doc($$"""
#Sum[建註冊器的擴展方法。]
#Descr[
三個重載按「知道多少元數據」區分，
常用的在最前面。
]
""")]
public static partial class ExtnITestFnRegister{
	[Doc($$"""
#Sum[建一個註冊器，測試類類型取泛型參數。]
#Descr[
*一個文件只測一個被測函數*時用這個重載最省事：
被測類型與被測函數名各給一個，
不必寫列表初始化器。

例：
```cs
var R = Node.{{nameof(ExtnITestFnRegister.MkRegister)}}<TestCalculator>(
	typeof(ICalculator),
	nameof(ICalculator.Add),
	"Add:"
).{{nameof(ITestFnRegister.Register)}};
```

*不給元數據時一個參數都不要傳*：
寫 `{{nameof(ExtnITestFnRegister.MkRegister)}}<TestX>()`，
不要寫 `{{nameof(ExtnITestFnRegister.MkRegister)}}<TestX>(null, null)`——
後者與另一個重載歧義，
編譯不過（`CS0121`）。
]
#TParams([測試類類型，須實作 `{{nameof(ITester)}}`])
#Params([[要掛用例的節點],
	[被測類型；雖是列表，但這個重載一般只傳一個],
	[被測函數名；一般只傳一個],
	[用例名前綴，默認不加]])
#Rtn[註冊器；取它的 `{{nameof(ITestFnRegister.Register)}}` 方法組即可註冊用例]
""")]
	public static partial ITestFnRegister MkRegister<TTester>(
		this ITestNode z
		,IList<Type>? TesteeTypes = null
		,IList<str>? TesteeFnNames = null
		,str UniqNamePrefix = ""
	);

	[Doc($$"""
#Sum[建一個註冊器，測試類類型也由調用方顯式給。]
#Descr[
只在無法用泛型參數給出測試類時使用
（例如測試類類型是運行時算出來的）。
]
#Params([[要掛用例的節點],
	[測試類類型],
	[被測類型列表],
	[被測函數名列表],
	[用例名前綴]])
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
	#Sum[只用一個被測函數名建註冊器。]
	#Descr[
	「一個文件只測一個被測函數」時最貼合的寫法。
	]
	#TParams([測試類類型，須實作 `{{nameof(ITester)}}`])
	#Params([[要掛用例的節點],
		[被測類型],
		[被測函數名，必須用 `nameof`],
		[用例名前綴，默認不加]])
	#Rtn[註冊器]
	""")]
	public static partial ITestFnRegister MkRegister<TTester>(
		this ITestNode z
		,Type TesteeType
		,str TesteeFnName
		,str UniqNamePrefix = ""
	);
}
