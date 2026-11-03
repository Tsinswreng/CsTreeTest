using Microsoft.Extensions.DependencyInjection;
using SampleDomain;
using Tsinswreng.CsTreeTest;

namespace SampleCart.StyleA;

/// <summary>樣本 A 的入口：裝配依賴 → 跑。兩步，沒有 <c>Init()</c>。</summary>
public static class Program{
	/// <summary>跑全部用例，失敗則回傳非零退出碼。</summary>
	public static async Task<int> Main(){
		// 第一步：依賴來源。
		// 不再有「必須先於 Init」這條規矩——
		// 依賴是每條用例執行時才取的，建樹過程一條都不取。
		SvcProvider = SampleApp.MkServices().BuildServiceProvider();

		// 第二步：跑。第一次讀 Root 時樹才建好（建樹過程不等任何東西）。
		var Mgr = CartTestMgr.Inst;
		var Summary = await new TreeTestExecutor().RunEtPrint(Mgr.Root);
		return Summary.Failed > 0 ? 1 : 0;
	}
}
