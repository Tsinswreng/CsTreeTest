using System.Collections.Concurrent;

using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：TreeTestExecutor 的成員宣告；實現見 TreeTestExecutor.Impl.cs。
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
