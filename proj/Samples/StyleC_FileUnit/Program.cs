using Microsoft.Extensions.DependencyInjection;
using SampleDomain;
using Tsinswreng.CsTreeTest;

namespace SampleCart.StyleC;

/// <summary>樣本 C 的入口。與 A、B 一樣是兩步。</summary>
public static class Program{
	/// <summary>跑全部用例，失敗則回傳非零退出碼。</summary>
	public static async Task<int> Main(){
		SvcProvider = SampleApp.MkServices().BuildServiceProvider();

		var Mgr = CartTestMgr.Inst;
		// 樹上沒有「測試類」那一層，路徑是
		// Tests/Cart/AddItem/AddNewLine
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
