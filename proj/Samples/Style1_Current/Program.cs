using Microsoft.Extensions.DependencyInjection;
using SampleCart.Style1.Cart;
using SampleDomain;
using Tsinswreng.CsTreeTest;

namespace SampleCart.Style1;

/// <summary>現行寫法的入口：裝配依賴 → <c>Init()</c> → 跑。</summary>
public static class Program{
	/// <summary>跑全部用例，失敗則回傳非零退出碼。</summary>
	public static async Task<int> Main(){
		// 第一步：依賴來源。必須在第二步之前——
		// 第二步會 new 出測試類，而測試類在建構時就取依賴。
		SvcProvider = SampleApp.MkServices().BuildServiceProvider();

		// 第二步：初始化，測試類此刻被實例化並掛用例。
		var Mgr = CartTestMgr.Inst;
		Mgr.Init();

		// 第三步：跑。
		var Summary = await new TreeTestExecutor().RunEtPrint(Mgr.Root);
		return Summary.Failed > 0 ? 1 : 0;
	}
}
