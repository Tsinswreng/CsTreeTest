using Microsoft.Extensions.DependencyInjection;
using SampleCart.Style0.Cart;
using SampleDomain;
using Tsinswreng.CsTreeTest;

namespace SampleCart.Style0;

/// <summary>
/// 舊版入口：裝配容器 → <c>InitSvc</c>（順帶建樹）→ 跑。
/// </summary>
public static class Program{
	/// <summary>跑全部用例，失敗則回傳非零退出碼。</summary>
	public static async Task<int> Main(){
		// 舊版寫法：測試類本身也在容器裏，因為它的依賴走建構子注入。
		var SvcColct = SampleApp.MkServices();
		SvcColct.AddSingleton<TestCart>();

		var Mgr = CartTestMgr.Inst;
		// 這一句同時做兩件事：建容器、建樹。所以兩者的順序在這裡是「一個 API 裏的事」。
		Mgr.InitSvc(SvcColct, sc => sc.BuildServiceProvider());

		var Summary = await new TreeTestExecutor().RunEtPrint(Mgr.TestNode);
		return Summary.Failed > 0 ? 1 : 0;
	}
}
