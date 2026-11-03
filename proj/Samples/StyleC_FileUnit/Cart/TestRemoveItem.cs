using SampleDomain.Cart;
using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Drafts;

namespace SampleCart.StyleC.Cart;

/// <summary>只測 ISvcCart.RemoveItem 的一個檔案。</summary>
public static class TestRemoveItem{
	/// <summary>把本檔的用例掛到給定節點上。</summary>
	/// <param name="Node">父節點（領域分組）。</param>
	public static void Register(ITestNode Node){
		var G = Node.Group(typeof(ISvcCart), nameof(ISvcCart.RemoveItem));
		G.Case(nameof(RemoveExistingReturnsQty), RemoveExistingReturnsQty!);
		G.Case(nameof(RemoveMissingReturnsZero), RemoveMissingReturnsZero!);
	}

	/// <summary>移除已存在的品項，回傳被移除的數量。</summary>
	static async Task<nil> RemoveExistingReturnsQty(obj? O){
		var Svc = GetRSvc<ISvcCart>();
		await Svc.AddItem("sc-rm-hit", 4);
		var Removed = await Svc.RemoveItem("sc-rm-hit");
		Assert.IsTrue(Removed == 4, $"期望 4，實際 {Removed}");
		Assert.IsTrue(await Svc.GetItem("sc-rm-hit") is null, "移除後不應再查得到");
		return NIL;
	}

	/// <summary>移除不存在的品項，回傳 0。</summary>
	static async Task<nil> RemoveMissingReturnsZero(obj? O){
		var Svc = GetRSvc<ISvcCart>();
		var Removed = await Svc.RemoveItem("sc-rm-none");
		Assert.IsTrue(Removed == 0, $"期望 0，實際 {Removed}");
		return NIL;
	}
}
