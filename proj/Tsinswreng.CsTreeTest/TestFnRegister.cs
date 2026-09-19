using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

[Doc("""
#Sum[往測試樹的一個節點上註冊用例的註冊器。]
#Descr[
同一個測試類的各條用例共享同一批元數據
（屬於哪個測試類、測哪些被測類型與被測函數），
所以元數據只在建註冊器時交一次，
之後每條用例只寫用例名與用例體。

正常用法不必直接碰本接口：
用擴展方法 `MkRegister` 造出來，
直接取它的 `Register` 方法組來連註冊多條用例：

```cs
var R = Node.MkRegister<TestCalculator>(
	typeof(ICalculator),
	nameof(ICalculator.Add),
	"Add:"
).Register;

R("正數相加", async (o) => { /* ... */ return NIL; });
R("負數相加", async (o) => { /* ... */ return NIL; });
```

*為甚麼要取方法組*
（`.Register` 而不是把註冊器對象存進局部變量）：
C# 只允許直接調用委託或成員訪問，
把註冊器存進局部變量後再 `R("名", fn)` 會編譯不過
（`CS0149：應輸入方法名稱`）。
]
""")]
public interface ITestFnRegister{
	[Doc("""
#Sum[註冊一條用例。]
#Descr[
用例的元數據取本註冊器*當前*的欄位值，
用例名為 `UniqNamePrefix + UniqName`。

由於元數據是註冊時快照的，
可以在兩條用例之間改本註冊器的欄位
（例如換一個前綴），
後面的用例就會用新值。

調用方式見接口註釋：
先 `var R = Node.MkRegister<...>(...).Register;`
再 `R("用例名", 用例體)`。
]
#Params([[用例名，不含前綴；應能看出所測行爲，禁止 `Test1`、`Try`、`Temp` 這類名字],
	[用例函數本體，簽名為 `Task<obj?> (obj? Arg)`]])
""")]
	void Register(str UniqName, FnTest Fn);

	[Doc("""
#Sum[本節點下的用例屬於哪個測試類。]
#Descr[
由 `MkRegister<TTester>` 的泛型參數寫入。
]
""")]
	Type TesterType{get;set;}

	[Doc("""
#Sum[本節點下的用例測的是哪些被測類型。]
""")]
	IList<Type> TesteeTypes{get;set;}

	[Doc("""
#Sum[本節點下的用例測的是哪些被測函數名。]
""")]
	IList<str> TesteeFnNames{get;set;}

	[Doc("""
#Sum[自動加在用例名前面的前綴。]
#Descr[
同一個被測類的用例散在多個節點時，
用前綴區分，
使全樹的用例名不重複。

例：測 `Add` 的文件用 `"Add:"`，
失敗輸出就會是
`Tests/Calculator/TestCalculator/Add:正數相加`。
]
""")]
	str UniqNamePrefix{get;set;}
}

[Doc("""
#Sum[註冊器的默認實現。]
#Descr[
一般不用直接 new；
用 `MkRegister` 擴展方法造。
]
""")]
public class TestFnRegister : ITestFnRegister{
	[Doc(@"#Sum[用例要掛上去的那個節點。]")]
	private readonly ITestNode Node;

	[Doc(@"#Sum[所屬測試類類型。]")]
	public Type TesterType{get;set;}

	[Doc(@"#Sum[被測類型列表。]")]
	public IList<Type> TesteeTypes{get;set;}

	[Doc(@"#Sum[被測函數名列表。]")]
	public IList<str> TesteeFnNames{get;set;}

	[Doc(@"#Sum[用例名前綴。]")]
	public str UniqNamePrefix{get;set;}

	[Doc("""
	#Sum[造一個註冊器。]
	#Descr[各元數據直接寫入本對象，之後 `Register` 時逐條快照給用例。]
	#Params([[用例要掛上去的節點],
		[所屬測試類類型],
		[被測類型列表],
		[被測函數名列表],
		[用例名前綴]])
	""")]
	public TestFnRegister(
		ITestNode Node
		,Type TesterType
		,IList<Type> TesteeTypes
		,IList<str> TesteeFnNames
		,str UniqNamePrefix
	){
		this.Node = Node;
		this.TesterType = TesterType;
		this.TesteeTypes = TesteeTypes;
		this.TesteeFnNames = TesteeFnNames;
		this.UniqNamePrefix = UniqNamePrefix;
	}

	[Doc("""
	#Sum[註冊一條用例：按當前元數據造用例並掛成子節點。]
	#Descr[
	用例名是 `UniqNamePrefix + UniqName`，
	節點名與用例名相同。
	]
	#Params([[用例名，不含前綴],[用例函數本體]])
	""")]
	public void Register(str UniqName, FnTest Fn){
		var Case = new TestCase{
			UniqName = UniqNamePrefix + UniqName,
			TesterType = TesterType,
			TesteeTypes = TesteeTypes,
			TesteeFnNames = TesteeFnNames,
			FnTest = Fn,
		};
		this.Node.Children.Add(new TestNode{
			UniqName = Case.UniqName,
			Data = Case,
			Parents = [this.Node],
		});
	}
}

[Doc("""
#Sum[建註冊器的擴展方法。]
#Descr[
三個重載按「知道多少元數據」區分，
常用的在最前面。
]
""")]
public static partial class ExtnITestFnRegister{
	[Doc("""
#Sum[建一個註冊器，測試類類型取泛型參數。]
#Descr[
*一個文件只測一個被測函數*時用這個重載最省事：
被測類型與被測函數名各給一個，
不必寫列表初始化器。

例：
```cs
var R = Node.MkRegister<TestCalculator>(
	typeof(ICalculator),
	nameof(ICalculator.Add),
	"Add:"
).Register;
```

*不給元數據時一個參數都不要傳*：
寫 `MkRegister<TestX>()`，
不要寫 `MkRegister<TestX>(null, null)`——
後者與另一個重載歧義，
編譯不過（`CS0121`）。
]
#TParams([測試類類型])
#Params([[要掛用例的節點],
	[被測類型；雖是列表，但這個重載一般只傳一個],
	[被測函數名；一般只傳一個],
	[用例名前綴，默認不加]])
#Rtn[註冊器；取它的 `Register` 方法組即可註冊用例]
""")]
	public static ITestFnRegister MkRegister<TTester>(
		this ITestNode z
		,IList<Type>? TesteeTypes = null
		,IList<str>? TesteeFnNames = null
		,str UniqNamePrefix = ""
	){
		return new TestFnRegister(
			z
			,typeof(TTester)
			,TesteeTypes ?? []
			,TesteeFnNames ?? []
			,UniqNamePrefix
		);
	}

	[Doc("""
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
	public static ITestFnRegister MkRegister(
		this ITestNode z
		,Type TesterType
		,IList<Type>? TesteeTypes = null
		,IList<str>? TesteeFnNames = null
		,str UniqNamePrefix = ""
	){
		return new TestFnRegister(
			z
			,TesterType
			,TesteeTypes ?? []
			,TesteeFnNames ?? []
			,UniqNamePrefix
		);
	}

	[Doc("""
	#Sum[只用一個被測函數名建註冊器。]
	#Descr[
	「一個文件只測一個被測函數」時最貼合的寫法。
	]
	#TParams([測試類類型])
	#Params([[要掛用例的節點],
		[被測類型],
		[被測函數名，必須用 `nameof`],
		[用例名前綴，默認不加]])
	#Rtn[註冊器]
	""")]
	public static ITestFnRegister MkRegister<TTester>(
		this ITestNode z
		,Type TesteeType
		,str TesteeFnName
		,str UniqNamePrefix = ""
	){
		return new TestFnRegister(
			z
			,typeof(TTester)
			,[TesteeType]
			,[TesteeFnName]
			,UniqNamePrefix
		);
	}
}