using SampleDomain;
using SampleDomain.Cart;
using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Drafts;

namespace SampleCart.StyleB.Cart;

// 只測 ISvcCart.AddItem 的一個檔案。
public partial class TestCart{
	/// <summary>註冊 AddItem 的用例。</summary>
	/// <param name="Node">本測試類的節點。</param>
	static void RegisterAddItem(ITestNode Node){
		// 元數據只在這裡寫一次：測 ISvcCart.AddItem。
		// 之後每條用例只寫名字與函數體；不用前綴，路徑已含分組名。
		var G = Node.Group<TestCart>(typeof(ISvcCart), nameof(ISvcCart.AddItem));

		// 同一被測函數的用例若要共用一份狀態，就標在分組這一層：
		// 只影響本函數的用例，不會把整個測試類都串起來。
		G.Node.Ordered = true;

		// 用例用常態具名函數，名字由 nameof 帶出，改名即編譯錯。
		G.Case(nameof(AddNewLine), AddNewLine!);
		G.Case(nameof(AddSameSkuAccumulates), AddSameSkuAccumulates!);
		G.Case(nameof(AddNonPositiveThrows), AddNonPositiveThrows!);
	}

	/// <summary>加入一個新品項。</summary>
	static async Task<nil> AddNewLine(obj? O){
		var Svc = GetRSvc<ISvcCart>();
		await Svc.AddItem("sb-add-new", 3);
		var Line = await Svc.GetItem("sb-add-new");
		Assert.IsTrue(Line?.Qty == 3, "期望數量 3");
		return NIL;
	}

	/// <summary>同一品項再加一次會累加。</summary>
	static async Task<nil> AddSameSkuAccumulates(obj? O){
		var Svc = GetRSvc<ISvcCart>();
		await Svc.AddItem("sb-add-acc", 2);
		await Svc.AddItem("sb-add-acc", 5);
		var Line = await Svc.GetItem("sb-add-acc");
		Assert.IsTrue(Line?.Qty == 7, "期望數量 7");
		return NIL;
	}

	/// <summary>數量非正會拋。</summary>
	static async Task<nil> AddNonPositiveThrows(obj? O){
		var Svc = GetRSvc<ISvcCart>();
		await Chk.Throws<ArgumentOutOfRangeException>(
			() => Svc.AddItem("sb-add-bad", 0)
		);
		return NIL;
	}
}
