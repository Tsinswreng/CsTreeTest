using Microsoft.Extensions.DependencyInjection;
using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test;

[Doc($$"""
#Sum[本框架自身測試的入口。]

#Descr[
步驟與真實測試工程完全一致：
+ 組裝容器（本工程用真的微軟依賴注入）並交給全局依賴來源；
+ 初始化測試管理器（測試類此刻才被實例化）；
+ 執行全部用例，並按失敗數決定進程退出碼。

本工程是測試程序集，
不受「框架不依賴依賴注入」的限制——
框架不引用依賴注入，但測試工程自己引用；
這樣才能真真切切走一遍「測試類向全局依賴來源取被測對象」的路。

`{{nameof(ExtnTreeTestExecutor.RunEtPrint)}}` 默認不拋異常，
所以「失敗即非零退出碼」是入口的義務：
本入口用返回值實現（等價於 `if(S.Failed > 0){ Environment.Exit(1); }`）。
]
""")]
internal class Program{
	[Doc($$"""
#Sum[入口。]

#Descr[
返回值作為進程退出碼：有失敗用例時為 1。
入口由 `proj/Tsinswreng.CsTreeTest.Scripts/Test.Impl.cs` 以 `dotnet run` 調起。
]
#Params([[命令行參數，本入口不使用]])
#Rtn[進程退出碼]
""")]
	public static async Task<int> Main(string[] args){
		// 步驟1：組裝容器，註冊本工程測試要用的被測服務。
		// 必須在步驟2之前：測試類的欄位初始化會立刻取依賴。
		var SvcColct = new ServiceCollection();
		SvcColct.AddSingleton<IFakeSvc, FakeSvc>();
		Di.SvcProvider = SvcColct.BuildServiceProvider();

		// 步驟2：初始化。測試類此刻被實例化並把用例掛到樹上。
		var Mgr = CsTreeTestTestMgr.Inst;
		Mgr.Init();

		// 步驟3：跑全部用例，按失敗數決定退出碼。
		var Executor = new TreeTestExecutor();
		var Summary = await Executor.RunEtPrint([Mgr.Root]);
		return Summary.Failed > 0 ? 1 : 0;
	}
}