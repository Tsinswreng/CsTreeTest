using System.Collections.Concurrent;
using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：屬性 + 構造函數聲明（partial method）；實現見 TreeTestExecutor.Impl.cs。

[Doc($$"""
#Sum[單條用例的運行結果。]
#Descr[
一次執行會為每條被跑到的用例造一個本對象。

*失敗不中斷*：
單條用例拋出的異常被收進 `Exception` 字段，
不會往外拋、也不會影響同批的其它用例，
所以每條被跑到的用例在 `{{nameof(TestRunSummary.Results)}}` 裏一定有且只有一條結果。
]
""")]
public sealed class TestCaseRunResult{
	[Doc($$"""
#Sum[該用例在本次執行中的次序。]
#Descr[
由*收集階段*按先序（父在前、同層按聲明序）逐項分配，
從 0 開始單調遞增，跨多個根節點也共用同一個計數器。

*為甚麼要這個字段*：
用例是並行跑的，完成順序隨機；
`{{nameof(TreeTestExecutor)}}` 的執行流程最後按本字段排序後才返回，
使用者看到的 `{{nameof(TestRunSummary.Results)}}` 因此與「樹的先序」一致，
兩次執行的輸出才可穩定對比。
]
""")]
	public required int Order{get;set;}

	[Doc($$"""
#Sum[該用例在樹上的位置，是一個*名字路徑*。]
#Descr[
由各祖先節點（含本節點）的唯一名以 `/` 拼成，例如
`Tests/Calculator/TestCalculator/Add:正數相加`。
在收集階段由 `{{nameof(ExtnITestNode.PathString)}}` 沿父節點上溯算好。

*為甚麼用名字路徑*：失敗輸出要給人看，
名字路徑一眼就能看出是哪個測試類的哪條用例，
比一串節點標識可讀得多。

*已知取捨*：這是名字路徑而非節點標識路徑，
同名節點下的路徑可能不唯一。
本字段只用於人看，
不要拿它當唯一鍵去比對用例；
要拿節點請用 `Node`。
]
""")]
	public required str NodePath{get;set;}

	[Doc($$"""
#Sum[該用例所在的樹節點。]
#Descr[
`{{nameof(TestCaseRunResult.NodePath)}}` 就是從本節點沿父節點上溯算出來的，
需要節點上的其它信息（`{{nameof(ITestNode.Children)}}`、`{{nameof(ITestNode.Parents)}}`、`Data` 等）時從這裏取。
]
""")]
	public required ITestNode Node{get;set;}

	[Doc($$"""
#Sum[該用例本身。]
#Descr[
失敗輸出時從它取 `{{nameof(ITesterNode.TesterType)}}` 與 `{{nameof(ITestNode.UniqName)}}`（見 `{{nameof(TreeTestExecutor)}}` 的失敗輸出），
所以結果對象不必再另存一份用例名。
]
""")]
	public required ITestCase TestCase{get;set;}

	[Doc(@$"#Sum[是否通過。用例函數正常返回即為真；拋出任何異常即為假。]")]
	public required bool IsPassed{get;set;}

	[Doc($$"""
#Sum[失敗時的例外；通過時為空。]
#Descr[
*用例拋出的異常只被收下、不往外拋*，
這是「失敗不中斷其餘用例」的落點。

是否要有失敗就讓進程失敗，
由入口按 `{{nameof(TestRunSummary.Failed)}}` 自行判斷；
本庫自己不做這個決定（見 `{{nameof(ExtnTreeTestExecutor.RunEtPrint)}}` 的 `ThrowOnFailed`）。
]
""")]
	public Exception? Exception{get;set;}

	[Doc(@$"#Sum[本條用例的耗時。只含用例函數體自身的執行時間，不含收集與匯總的時間。]")]
	public TimeSpan Elapsed{get;set;}
}

[Doc($$"""
#Sum[一次執行的匯總統計。]
#Descr[
不管有沒有失敗，`{{nameof(ITestExecutor.Run)}}` 都返回本對象；
失敗的詳情在 `{{nameof(TestRunSummary.Results)}}` 裏逐條看（或直接用 `{{nameof(TestRunSummary.FailedResults)}}()`）。
]
""")]
public sealed partial class TestRunSummary{
	[Doc(@$"#Sum[本次執行的開始時間。在收集階段之前記下，所以收集耗時也計在總耗時裏。]")]
	public DateTimeOffset StartedAt{get;set;}

	[Doc(@$"#Sum[本次執行的結束時間。所有用例跑完並匯總完之後記下。]")]
	public DateTimeOffset EndedAt{get;set;}

	[Doc(@$"#Sum[總耗時，即 `{nameof(EndedAt)} - {nameof(StartedAt)}`（按需算，不是存下來的字段）。]")]
	public TimeSpan Elapsed => EndedAt - StartedAt;

	[Doc(@$"#Sum[本次實際跑到的用例數，即 `{nameof(Results)}.Count`。沒有被掃到的節點不計。]")]
	public int Total{get;set;}

	[Doc(@$"#Sum[通過數。]")]
	public int Passed{get;set;}

	[Doc(@$"#Sum[失敗數，即 `{nameof(Total)} - {nameof(Passed)}`。入口通常就靠它決定進程成敗。]")]
	public int Failed{get;set;}

	[Doc($$"""
#Sum[逐條用例的結果，*按 `{{nameof(TestCaseRunResult.Order)}}`（樹的先序）排列*。]
#Descr[
不是按完成順序排列——
`{{nameof(TreeTestExecutor)}}` 的執行流程返回前已按 `{{nameof(TestCaseRunResult.Order)}}` 排過序，
所以並行與否都不影響這裏的順序。
]
""")]
	public IList<TestCaseRunResult> Results{get;set;} = [];

	[Doc($$"""
#Sum[只取失敗那幾條（保持 `{{nameof(TestRunSummary.Results)}}` 的順序）。]
#Descr[
供調用方自行出報告時用，
不必自己去 `{{nameof(TestRunSummary.Results)}}` 裏篩。

注意本方法會把結果複製成新列表，
所以不要在熱路徑上反覆調用。
]
#Rtn[失敗用例的結果列表；全部通過時為空列表]
""")]
	public partial IList<TestCaseRunResult> FailedResults();
}

[Doc($$"""
#Sum[有失敗時拋出的例外，異常消息即匯總。]
#Descr[
*不是框架主動拋的*：
`Run` 與 `{{nameof(ExtnTreeTestExecutor.RunTests)}}` 從不因為用例失敗而拋；
只有 `{{nameof(ExtnTreeTestExecutor.RunEtPrint)}}` 的 `ThrowOnFailed` 為真、且 `Summary.Failed > 0` 時才拋本例外。

本例外不帶任何「哪條用例失敗」的信息，
逐條失敗原因在 `Summary.Results` 裏各條的 `Exception`——
本例外只是一個「這次執行有失敗」的信號。

是否要拋由調用方決定：
本庫默認只跑與報告，
進程的成敗由入口自己判斷。
]
""")]
public sealed partial class TestRunFailedException : Exception{
	[Doc($$"""
#Sum[產生本異常的那次執行的匯總。]
#Descr[
與異常消息同源（消息就是按它的三個計數拼的）；
需要逐條失敗詳情時用它，
不要去解析異常消息字符串。
]
""")]
	public TestRunSummary Summary{get;}

	[Doc($$"""
#Sum[用匯總造例外，消息為「總數／通過／失敗」。]
#Descr[
消息格式固定為
`測試有失敗。總數={Total}，通過={Passed}，失敗={Failed}`。

`Summary` 是*原樣*記下來的引用，不會複製，
所以拿到異常之後仍能訪問逐條結果。
]
#Params([[產生本異常的那次執行的匯總]])
""")]
	public partial TestRunFailedException(TestRunSummary Summary);
}

[Doc($$"""
#Sum[執行選項。]
#Descr[
三個字段都是公開*字段*（不是屬性），
直接寫字段初始化器即可，
只改關心的那幾個：

```cs
await new TreeTestExecutor().RunEtPrint(root, Opt: new OptTestExecutor{
	IsParallel = false,
	MaxDegreeOfParallelism = 4,
	Arg = myFixture,
});
```

`{{nameof(ITestExecutor.Run)}}` 的 `Opt` 傳空時會自動用 `new OptTestExecutor()`，
所以本類型的所有默認值就是框架的默認行為。
]
""")]
public class OptTestExecutor{
	[Doc($$"""
#Sum[是否並行；默認 `true`。]
#Descr[
為真時同一階段內*可並行*的項走 `Parallel.ForEachAsync` 同時跑；
為假時整棵樹退化成單線程按收集順序（先序）跑，
`{{nameof(OptTestExecutor.MaxDegreeOfParallelism)}}` 也一併失效。

*這是全局總閘*：
`{{nameof(ITestNode.IsParallelRecursive)}}` 是逐子樹的閘，兩者互相獨立、可同時使用；
一項只要*任一*閘要求不並行，它就不並行。
]
""")]
	public bool IsParallel = true;

	[Doc($$"""
#Sum[最大同時執行數；默認 `Environment.ProcessorCount`。]
#Descr[
原樣傳給 `ParallelOptions.MaxDegreeOfParallelism`。

`<= 0` 時同樣退回到 `Environment.ProcessorCount`（見 `{{nameof(TreeTestExecutor.Run)}}`），
所以*不要用 0 或負數表示「不並行」*——
要整個執行不並行請用 `IsParallel = false`。

只有 `{{nameof(OptTestExecutor.IsParallel)}}` 為真時本字段才有意義。
]
""")]
	public int MaxDegreeOfParallelism = Environment.ProcessorCount;

	[Doc($$"""
#Sum[傳給用例函數的參數；默認空。]
#Descr[
會*原樣*傳給每條用例的函數簽名最後一個參數（即 `{{nameof(FnTest)}}` 的 `Arg`），
所有用例拿到的是同一個引用，
用例不需要時忽略即可。

注意優先級：
擴展方法 `{{nameof(ExtnTreeTestExecutor.RunTests)}}` / `{{nameof(ExtnTreeTestExecutor.RunEtPrint)}}` 的 `Arg` 參數只在*本字段為空*時才寫進來
（`Opt.Arg ??= Arg`），
即本字段優先。
]
""")]
	public obj? Arg = default;
}

[Doc($$"""
#Sum[測試執行器：跑一組節點，返回匯總。]
#Descr[
正常用法不必直接調 `Run`：
用 `{{nameof(ExtnTreeTestExecutor)}}` 的 `{{nameof(ExtnTreeTestExecutor.RunTests)}}` / `{{nameof(ExtnTreeTestExecutor.RunEtPrint)}}` 更方便
（可傳單個節點、會補默認選項、還能順帶打印）。
]
""")]
public interface ITestExecutor{
	[Doc($$"""
#Sum[跑一組節點。]
#Descr[
一次可給多個節點，
各節點掃描出來的用例*合併成同一組結果*、按 `{{nameof(TestCaseRunResult.Order)}}` 排序後返回——
所以既可以傳根跑整棵樹，
也可以只傳幾個子樹跑一部分。

*本方法不打印、也不因用例失敗而拋*：
失敗被收進 `{{nameof(TestCaseRunResult.Exception)}}`，
是否需要「有失敗就失敗」由調用方按 `{{nameof(TestRunSummary.Failed)}}` 判斷。

`Nodes` 為空列表時什麼都不跑，返回一個 `Total = 0` 的匯總（不拋）。
]
#Params([[要跑的根節點列表，不能為空引用；列表內元素的順序決定 `{{nameof(TestCaseRunResult.Order)}}` 的分配次序],
	[執行選項；傳空則用 `new OptTestExecutor()` 的默認值]])
#Rtn[本次執行的匯總，`{{nameof(TestRunSummary.Results)}}` 已按 `{{nameof(TestCaseRunResult.Order)}}` 排序]
""")]
	Task<TestRunSummary> Run(
		IList<ITestNode> Nodes
		,OptTestExecutor? Opt
	);
}

[Doc($$"""
#Sum[測試執行器的默認實現。]
#Descr[
*執行模型：按層次分階段（stage）推進，階段之間有屏障。*

收集階段（`{{nameof(CollectWorkStages)}}` / `{{nameof(CollectNodeStages)}}`）把整棵樹攤平成一串*階段*：
同一階段內的項彼此沒有依賴，可以同時跑；
進入下一階段前必須等本階段全部跑完——
這就是「跨階段屏障」，
也是「先插測試數據、再測、最後清理」得以成立的原因。
樹有多深階段就有多少個；
`{{nameof(ITestNode.Ordered)}}` 的子樹會把階段接成一條更長的串行鏈。

跑階段時（`{{nameof(RunCore)}}`）同一階段內分兩批：
先跑*可並行*的項（`Parallel.ForEachAsync`，受 `{{nameof(OptTestExecutor.MaxDegreeOfParallelism)}}` 限制），
再*依序*跑被標記為*不可並行*的項。
所以「不可並行」的語義是「不與其它項同時跑」，
而不是「插在並行批之前」。

*兩個控制執行方式的節點開關互相獨立，不要混用*（詳見 `{{nameof(ITestNode)}}`）：
+ `{{nameof(ITestNode.Ordered)}}`：
	只管*直接子節點*之間的先後次序，
	*不遞歸*——
	單個子節點內部仍然可以並行。
+ `{{nameof(ITestNode.IsParallelRecursive)}}`：
	為真時*本節點及其全部遞歸子節點*都不參與並行
	（即只與兄弟依序輪流、不與別的分支同時跑），
	且本聲明*向下繼承*，標在最外層即可約束整棵子樹。

*失敗不中斷*：
單條用例拋出的異常被收進 `{{nameof(TestCaseRunResult.Exception)}}`，
其餘用例照跑；
本類只在調用方用錯（如 `Nodes` 為空引用）時才拋。

*結果按 `{{nameof(TestCaseRunResult.Order)}}` 排序後才返回*：
`{{nameof(TestCaseRunResult.Order)}}` 是收集階段按先序分配的，
而並行完成的順序是隨機的，
不排序就得不到穩定的輸出順序。
]
""")]
public sealed partial class TreeTestExecutor : ITestExecutor{
	[Doc($$"""
#Sum[一次執行的最小單位：一條用例加它的執行元數據。]
#Descr[
收集階段就把「要跑什麼」「路徑」「是否不並行」都算好放進來，
跑階段因此不用再碰樹——
既避免並行時讀樹造成競爭，也省掉重複上溯。

`{{nameof(TestCaseRunResult.NodePath)}}` 只在收集時算一次（`{{nameof(ExtnITestNode.PathString)}}` 要沿父節點上溯）；
`{{nameof(ShouldNotParallelize)}}` 存的是*把繼承算完之後的結果*，
而不是本節點自己的 `{{nameof(ITestNode.IsParallelRecursive)}}`。
]
""")]
	private sealed class WorkItem{
		[Doc(@$"#Sum[先序序號，由收集階段共享的遞增計數器分配。]")]
		public required int Order{get;set;}

		[Doc(@$"#Sum[名字路徑，收集時由 `{nameof(ExtnITestNode.PathString)}` 算好，跑階段直接用。]")]
		public required str NodePath{get;set;}

		[Doc(@$"#Sum[用例所在的樹節點。]")]
		public required ITestNode Node{get;set;}

		[Doc(@$"#Sum[要跑的那條用例。]")]
		public required ITestCase TestCase{get;set;}

		[Doc($$"""
#Sum[本項是否不參與並行。]
#Descr[
*繼承已經算進去了*：
本節點或其任一祖先的 `{{nameof(ITestNode.IsParallelRecursive)}}` 為真，本值即為真。
]
""")]
		public required bool ShouldNotParallelize{get;set;}
	}

	[Doc($$"""
#Sum[跑一組節點。]
#Descr[
三步：收集階段（`{{nameof(CollectWorkStages)}}`）→ 跑階段（`{{nameof(RunCore)}}`）→ 匯總。

*只在這裏做參數兜底*：
`Nodes` 為空引用直接拋；
`Opt` 為空則用 `new OptTestExecutor()`。
]
#Params([[要跑的根節點列表，不能為空引用],
	[執行選項；傳空則用默認值]])
#Rtn[本次執行的匯總]
""")]
	public partial Task<TestRunSummary> Run(
		IList<ITestNode> Nodes,
		OptTestExecutor? Opt
	);

	[Doc($$"""
#Sum[按階段跑：逐個階段推進，階段之間有屏障。]
#Descr[
*每個階段內分兩批*：
先把可並行項用 `Parallel.ForEachAsync` 一起跑完，
再依序跑不可並行項。
「不可並行」= 不與其它項同時跑，
不是「插在並行批之前」。

*階段之間是屏障*：
本階段全部項跑完才進下一階段。

`{{nameof(OptTestExecutor.IsParallel)}}` 為假時不分批，
所有階段的所有項按收集順序（先序）單線程跑，
並行度失效。

不管走哪條分支，最後都*按 `{{nameof(TestCaseRunResult.Order)}}` 排序*再統計：
`{{nameof(TestRunSummary.Results)}}` 是並發寫入的 `ConcurrentBag`，
完成順序隨機，不排序就得不到穩定的輸出順序。
]
#Params([[開始時間，由 `Run` 記下，原樣寫進匯總],
	[收集階段算出的階段列表；下標即階段序號],
	[結果收集籃，並發安全],
	[並行選項，此處只用到並行度],
	[傳給每條用例的參數],
	[是否並行；為假則整棵樹單線程依序跑]])
#Rtn[本次執行的匯總；`{{nameof(TestRunSummary.Results)}}` 已按 `{{nameof(TestCaseRunResult.Order)}}` 排序]
""")]
	private static partial Task<TestRunSummary> RunCore(
		DateTimeOffset StartedAt,
		IList<IList<WorkItem>> WorkStages,
		ConcurrentBag<TestCaseRunResult> Results,
		ParallelOptions ParallelOptions,
		obj? Arg,
		bool IsParallel
	);

	[Doc($$"""
#Sum[執行單條用例：成功失敗都收成一條結果。]
#Descr[
*用例拋出的任何異常都被吞進 `Caught`，不往外拋*——
否則一條用例失敗會把同批並行的其餘用例一起帶走，
而「失敗不中斷其餘用例」是框架的既定語義。

成功與失敗走同一條出口，
所以 `{{nameof(TestRunSummary.Results)}}` 裏每條被跑到的用例都恰好有一條結果，
`{{nameof(TestRunSummary.Total)}}` 也因此等於實際跑過的用例數。

本方法不碰樹、不排序，
只依賴 `{{nameof(WorkItem)}}` 裏收集階段算好的元數據。
]
#Params([[要跑的項，含用例與執行元數據],
	[結果收集籃，並發安全],
	[傳給用例函數的參數]])
#Rtn[無；結果寫進 `{{nameof(TestRunSummary.Results)}}`]
""")]
	private static partial Task ExecuteItem(
		WorkItem Item,
		ConcurrentBag<TestCaseRunResult> Results,
		obj? Arg
	);

	[Doc($$"""
#Sum[多個根節點各自攤平成階段，再按階段序號合併。]
#Descr[
每個根節點單獨攤平（`{{nameof(CollectNodeStages)}}`），
然後*按階段序號對齊合併*：
各根的第 0 階段併成一個階段，
各根的第 1 階段併成一個階段……
於是多個根之間也享有同樣的階段屏障。

*`{{nameof(TestCaseRunResult.Order)}}` 是跨根共享的唯一計數器*，
按「根的先後 + 每個根的先序」依次分配，
所以合併之後仍可按 `{{nameof(TestCaseRunResult.Order)}}` 穩定排序。

根節點自身的 `{{nameof(ITestNode.IsParallelRecursive)}}` 仍然生效——
它由 `{{nameof(CollectNodeStages)}}` 內部按「父繼承或自身」算出，
所以這裏傳進去的 `false` 只表示「根之上沒有祖先可繼承」。
]
#Params([[要收集的根節點列表，按給定順序逐個處理]])
#Rtn[階段列表；下標即階段序號，每個階段內是要跑的項]
""")]
	private static partial IList<IList<WorkItem>> CollectWorkStages(IList<ITestNode> Nodes);

	[Doc($$"""
#Sum[收集單個節點的子樹，攤平成階段列表。]
#Descr[
三種情形：

+ *本節點帶用例*（`Data` 是 `{{nameof(ITestCase)}}`）：
	葉子。返回一個只含這一項的階段，收集到此為止。
+ *本節點 `{{nameof(ITestNode.Ordered)}}` 為真*：
	各子節點的階段列表*首尾相接*，
	於是子節點之間形成一條串行鏈。
	*只管直接子節點、不遞歸*：
	每個子節點內部仍然各自分階段、仍然可以並行——
	`{{nameof(ITestNode.Ordered)}}` 約束的是「直接子節點之間的先後」，
	不是「整棵子樹不並行」。
+ *其它*：
	各子節點的階段*按序號合併*，
	同一序號的項落在同一階段裡，
	於是同層的項落在同一個階段、因此可以並行。

`{{nameof(ITestNode.IsParallelRecursive)}}` 的處理：
本節點的 `{{nameof(WorkItem.ShouldNotParallelize)}}` 是
「父節點繼承下來的 `ParentIsParallelRecursive`」
或上「本節點自己的 `{{nameof(ITestNode.IsParallelRecursive)}}`」，
再原樣往所有子節點傳——
這就是*向下繼承*：標在最外層即可讓整棵子樹都不參與並行。
它與 `{{nameof(ITestNode.Ordered)}}` 互相獨立，兩者可以同時生效。
]
#Params([[當前節點],
	[當前節點的名字路徑，由父節點逐層拼好傳進來],
	[先序計數器，跨節點共享，用來分配 `{{nameof(TestCaseRunResult.Order)}}`],
	[從父節點繼承下來的不並行標記；根節點傳 `false`]])
#Rtn[本子樹的階段列表]
""")]
	private static partial IList<IList<WorkItem>> CollectNodeStages(
		ITestNode Node,
		str Path,
		ref int Order,
		bool ParentIsParallelRecursive
	);

	[Doc($$"""
#Sum[確保階段列表至少有 `Index + 1` 個階段，不足時補出空階段。]
#Descr[
合併多個子樹的階段時，
某個子樹可能比別的淺（階段數少），
直接按下標寫入會越界，
所以先補出空階段再寫。
補出來的是空列表，對執行沒有任何影響。
]
#Params([[階段列表，原地擴充],
	[需要的階段下標，從 0 開始]])
#Rtn[無]
""")]
	private static partial void EnsureStage(IList<IList<WorkItem>> Stages, int Index);
}

[Doc($$"""
#Sum[執行器的便捷入口。]
#Descr[
四個重載按兩件事組合：
*跑單個節點還是節點列表*（`{{nameof(ExtnTreeTestExecutor.RunTests)}}` 與 `{{nameof(ExtnTreeTestExecutor.RunEtPrint)}}` 各有兩種），
以及*要不要跑完打印*。

共同約定：
+ `{{nameof(ITestMgr.Root)}}` / `Nodes` 不能為空引用，為空直接拋 `ArgumentNullException`。
+ `Opt` 傳空時自動用 `new OptTestExecutor()`。
+ `Arg` 只在 `{{nameof(OptTestExecutor.Arg)}}` 為空時才寫進去（`Opt.Arg ??= Arg`），
	即 `{{nameof(OptTestExecutor.Arg)}}` 優先。
+ 單節點版的重載只是把節點包成單元素列表後轉發給列表版。
]
""")]
public static partial class ExtnTreeTestExecutor{
	[Doc($$"""
#Sum[跑一棵子樹：不打印、不拋例外，結果由調用方自己處理。]
#Descr[
想自己出報告、或想按 `Summary.Failed` 自定義成敗判斷時用這個。
]
#Params([[執行器，即 `this`],
	[子樹的根節點，不能為空引用],
	[傳給用例的參數；只在 `{{nameof(OptTestExecutor.Arg)}}` 為空時才生效],
	[執行選項；傳空則用默認值]])
#Rtn[本次執行的匯總；有失敗時也只返回匯總、不拋]
""")]
	public static partial Task<TestRunSummary> RunTests(
		this ITestExecutor z
		,ITestNode Root
		,obj? Arg = default
		,OptTestExecutor? Opt = default
	);

	[Doc($$"""
#Sum[跑一棵子樹，跑完按默認格式打印匯總。]
#Descr[
本重載只是把單節點包成單元素列表後轉發給列表版，
打印格式與拋例外邏輯都在列表版（見 `{{nameof(WriteSummary)}}`）。
]
#Params([[執行器，即 `this`],
	[子樹的根節點，不能為空引用],
	[傳給用例的參數；只在 `{{nameof(OptTestExecutor.Arg)}}` 為空時才生效],
	[執行選項；傳空則用默認值],
	[為真時，有失敗則拋 `{{nameof(TestRunFailedException)}}`；默認 `false` 不拋]])
#Rtn[本次執行的匯總；`ThrowOnFailed` 為真且有失敗時改為拋例外、不返回]
""")]
	public static partial Task<TestRunSummary> RunEtPrint(
		this ITestExecutor z
		,ITestNode Root
		,obj? Arg = default
		,OptTestExecutor? Opt = default
		,bool ThrowOnFailed = false
	);

	[Doc($$"""
#Sum[跑一組節點：不打印、不拋例外。]
#Descr[
想一次跑多棵子樹、又要自己處理結果時用這個。
多個節點的結果合併成同一份匯總、按 `{{nameof(TestCaseRunResult.Order)}}` 排序。
]
#Params([[執行器，即 `this`],
	[要跑的根節點列表，不能為空引用],
	[傳給用例的參數；只在 `{{nameof(OptTestExecutor.Arg)}}` 為空時才生效],
	[執行選項；傳空則用默認值]])
#Rtn[本次執行的匯總；有失敗時也只返回匯總、不拋]
""")]
	public static partial Task<TestRunSummary> RunTests(
		this ITestExecutor z
		,IList<ITestNode> Nodes
		,obj? Arg = default
		,OptTestExecutor? Opt = default
	);

	[Doc($$"""
#Sum[跑一組節點，跑完打印匯總，可選在有失敗時拋例外。]
#Descr[
*打印格式*（`{{nameof(WriteSummary)}}`）：
先逐條打印失敗用例（測試類／用例名／耗時／用例路徑／異常全文），
再打印匯總（總數、通過、失敗、耗時）。
*通過的用例不逐條打印*，
免得全綠時刷屏。

*`ThrowOnFailed` 的語義*：
+ 為假（默認）：就算全部失敗也正常返回匯總。
+ 為真且 `Summary.Failed > 0`：*打印完之後*才拋 `{{nameof(TestRunFailedException)}}`，
	異常裏帶着同一份匯總（`{{nameof(TestRunFailedException.Summary)}}`），
	消息即「總數／通過／失敗」。

默認不拋是有意的：
本庫只負責跑與報告，
*進程的成敗應由入口自己判斷*
（入口可能只要求某些子樹必須全過）。
]
#Params([[執行器，即 `this`],
	[要跑的根節點列表，不能為空引用],
	[傳給用例的參數；只在 `{{nameof(OptTestExecutor.Arg)}}` 為空時才生效],
	[執行選項；傳空則用默認值],
	[為真時，有失敗則拋 `{{nameof(TestRunFailedException)}}`；默認 `false` 不拋]])
#Rtn[本次執行的匯總；`ThrowOnFailed` 為真且有失敗時改為拋例外、不返回]
""")]
	public static partial Task<TestRunSummary> RunEtPrint(
		this ITestExecutor z
		,IList<ITestNode> Nodes
		,obj? Arg = default
		,OptTestExecutor? Opt = default
		,bool ThrowOnFailed = false
	);

	[Doc($$"""
#Sum[打印失敗用例與匯總。]
#Descr[
*只逐條打印失敗的用例*，通過的不打印；
失敗那幾條按 `{{nameof(TestCaseRunResult.Order)}}` 排序輸出，
所以輸出順序與樹的先序一致、可穩定對比。

每條失敗打三行：
`[失敗] 測試類短名/用例名 （耗時）`、
縮進的用例路徑、
以及異常的 `ToString()` 全文（深紅）。

匯總部分：
`[CsTreeTest]` 標題，
`總數`、`通過`（有通過時標綠）、`失敗`（有失敗時標紅）、`耗時`。
`TesterType` 為空時測試類名顯示為 `UnknownTester`。
]
#Params([[要打印的匯總]])
#Rtn[無；直接寫到控制台]
""")]
	private static partial void WriteSummary(TestRunSummary Summary);

	[Doc($$"""
#Sum[用指定顏色寫一段文字，寫完恢復原顏色。]
#Descr[
不改調用方的顏色設置：
先存下 `Console.ForegroundColor`，寫完再還原。
]
#Params([[要寫的文字],
	[前景色]])
#Rtn[無]
""")]
	private static partial void WriteWithColor(str Text, ConsoleColor Color);

	[Doc($$"""
#Sum[用指定顏色寫一行文字，寫完恢復原顏色。]
#Descr[
與 `{{nameof(WriteWithColor)}}` 同，只是多一個換行；
異常全文用本方法輸出。
]
#Params([[要寫的文字],
	[前景色]])
#Rtn[無]
""")]
	private static partial void WriteLineWithColor(str Text, ConsoleColor Color);
}