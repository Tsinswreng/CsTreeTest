using Microsoft.Extensions.DependencyInjection;
using MyProj1.Test;
using MyProj1.Test.MyDomains.Calculator;
using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace MyProj1.Test;

[Doc("""
#Sum[測試工程的入口。三步固定不變，順序不能顛倒。]

#Descr[
+ *第一步：組裝容器並交給全局依賴來源。*
	必須在第二步之前——
	測試類在欄位初始化時就向依賴來源取被測對象，
	而第二步會立刻實例化測試類。
+ *第二步：初始化測試管理器。*
	測試類此時才被 `new` 出來、
	把用例掛到樹上、
	索引此時建立。
+ *第三步：挑範圍執行。*
	要跑哪些用例*不需要命令行參數*，
	用管理器公開的查詢與篩選 API 在程式碼裏挑
	（見 `Main` 裏的四種寫法）。

返回值作爲進程退出碼：
`0` 全部通過，
`1` 有失敗用例。
]
""")]
public static class Program{
	[Doc(@"#Sum[服務容器；被測對象都在這裏註冊。]")]
	public static IServiceCollection SvcColct = new ServiceCollection();

	[Doc(@"#Sum[建好的服務提供器；賦給 `Di.SvcProvider` 後測試類纔能取到依賴。]")]
	public static IServiceProvider SvcProvdr = null!;

	[Doc("""
#Sum[入口。]

#Descr[
四種挑選執行範圍的寫法都寫在下面，
默認只跑最後一種（篩選器那條），
想看哪種就把其餘的注釋打開。
]
#Params([[命令行參數，本示例不使用：挑選範圍寫在程式碼裏]])
#Rtn[進程退出碼，有失敗用例時為 1]
""")]
	public static async Task<int> Main(string[] args){
		// 第一步：註冊被測對象，建好容器，交給全局依賴來源。
		SvcColct.AddSingleton<ICalculator, Calculator>();
		SvcProvdr = SvcColct.BuildServiceProvider();
		Di.SvcProvider = SvcProvdr;

		// 第二步：初始化測試管理器。
		// 測試類此刻被實例化，因此必須在第一步之後。
		var Mgr = MyProj1TestMgr.Inst;
		Mgr.Init();

		// 第三步：挑範圍執行。
		ITestExecutor Executor = new TreeTestExecutor();

		// 3.1 全部用例一鍵跑完。
		// await Executor.RunEtPrint(Mgr.Root);

		// 3.2 只跑某一個測試類。
		// await Executor.RunEtPrint(Mgr.GetNodeByTesterType(typeof(TestCalculator)));

		// 3.3 只跑測某一個被測函數的用例。
		// var ByFn = Mgr.GetNodesByTesteeFnName(nameof(ICalculator.Add));
		// await Executor.RunEtPrint(ByFn);

		// 3.4 用篩選器挑：
		// 測 ICalculator、且名字帶「正數」的用例。
		// 先看命中哪些，再跑。
		var Hits = Mgr.Filter()
			.TesteeType<ICalculator>()
			.NameContains("正數")
			.Select();
		Hits.PrintHits();
		var Summary = await Hits.RunEtPrint();
		return Summary.Failed > 0 ? 1 : 0;
	}
}