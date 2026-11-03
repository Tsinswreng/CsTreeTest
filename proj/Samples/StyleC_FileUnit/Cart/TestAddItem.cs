using SampleDomain;
using SampleDomain.Cart;
using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Drafts;

namespace SampleCart.StyleC.Cart;

/// <summary>只測 ISvcCart.AddItem 的一個檔案。整檔就這一個靜態類，沒有主檔。</summary>
public static class TestAddItem{
	/// <summary>把本檔的用例掛到給定節點上。</summary>
	/// <param name="Node">父節點（領域分組）。</param>
	public static void Register(ITestNode Node){
		var G = Node.Group(typeof(ISvcCart), nameof(ISvcCart.AddItem));
		G.Case(nameof(AddNewLine), AddNewLine!);
		G.Case(nameof(AddSameSkuAccumulates), AddSameSkuAccumulates!);
		G.Case(nameof(AddNonPositiveThrows), AddNonPositiveThrows!);
	}

	/// <summary>加入一個新品項。</summary>
	static async Task<nil> AddNewLine(obj? O){
		var Svc = GetRSvc<ISvcCart>();
		await Svc.AddItem("sc-add-new", 3);
		var Line = await Svc.GetItem("sc-add-new");
		Assert.IsTrue(Line?.Qty == 3, "期望數量 3");
		return NIL;
	}

	/// <summary>同一品項再加一次會累加。</summary>
	static async Task<nil> AddSameSkuAccumulates(obj? O){
		var Svc = GetRSvc<ISvcCart>();
		await Svc.AddItem("sc-add-acc", 2);
		await Svc.AddItem("sc-add-acc", 5);
		var Line = await Svc.GetItem("sc-add-acc");
		Assert.IsTrue(Line?.Qty == 7, "期望數量 7");
		return NIL;
	}

	/// <summary>數量非正會拋。</summary>
	static async Task<nil> AddNonPositiveThrows(obj? O){
		var Svc = GetRSvc<ISvcCart>();
		await Chk.Throws<ArgumentOutOfRangeException>(
			() => Svc.AddItem("sc-add-bad", 0)
		);
		return NIL;
	}
}
