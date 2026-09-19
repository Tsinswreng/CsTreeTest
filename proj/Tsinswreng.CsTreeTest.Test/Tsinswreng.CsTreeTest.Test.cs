using Microsoft.Extensions.DependencyInjection;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test;

/// 入口。
///
/// 步驟與真實測試工程一致：
/// 1. 組裝容器（微軟依賴注入）並交給全局依賴來源；
/// 2. 初始化測試管理器（測試類此時被實例化）；
/// 3. 執行並按失敗數決定退出碼。
internal class Program{
	public static async Task<int> Main(string[] args){
		// 步驟1：組裝容器，
		// 註冊本工程測試要用的被測服務。
		var SvcColct = new ServiceCollection();
		SvcColct.AddSingleton<IFakeSvc, FakeSvc>();
		Di.SvcProvider = SvcColct.BuildServiceProvider();

		// 步驟2：初始化。
		var Mgr = CsTreeTestTestMgr.Inst;
		Mgr.Init();

		// 步驟3：跑全部用例。
		var Executor = new TreeTestExecutor();
		var Summary = await Executor.RunEtPrint([Mgr.Root]);
		return Summary.Failed > 0 ? 1 : 0;
	}
}