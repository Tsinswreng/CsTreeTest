using Microsoft.Extensions.DependencyInjection;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test;

internal class Program{
	public static IServiceCollection SvcColct = new ServiceCollection();
	public static IServiceProvider SvcProvdr = null!;
	public static async Task Main(string[] args){
		// 測試對象全部是框架自身類型，無需註冊業務服務；tester 由 DI 解析（無參構造即可）。
		var mgr = CsTreeTestTestMgr.Inst;
		SvcProvdr = mgr.InitSvc(SvcColct, sc => sc.BuildServiceProvider());

		ITestExecutor executor = new TreeTestExecutor();
		// 慣例（2026-09-11 定）：RunEtPrint 默認不拋異常（避免冗餘堆棧），
		// 要非零退出碼就在入口自行 if 判斷——庫只管跑和報告，進程行為歸入口。
		var Summary = await executor.RunEtPrint(mgr.TestNode);
		if(Summary.Failed > 0){
			Environment.Exit(1);
		}
	}
}