using Microsoft.Extensions.DependencyInjection;
using SampleDomain;
using Tsinswreng.CsTreeTest;

namespace SampleCart.StyleB;

/// <summary>樣本 B 的入口。與樣本 A 一樣是兩步。</summary>
public static class Program{
	/// <summary>跑全部用例，失敗則回傳非零退出碼。</summary>
	public static async Task<int> Main(){
		SvcProvider = SampleApp.MkServices().BuildServiceProvider();

		var Mgr = CartTestMgr.Inst;
		// 想看樹長甚麼樣就打開這一行：
		// 每個被測函數多了一層分組節點，路徑變成
		// Tests/Cart/TestCart/AddItem/加入新條目
		foreach(var Node in Mgr.Root.Walk()){
			if(Node.Data is null){
				continue;
			}
			Console.WriteLine(Node.PathString());
		}

		var Summary = await new TreeTestExecutor().RunEtPrint(Mgr.Root);


		return Summary.Failed > 0 ? 1 : 0;
	}
}
