using SampleDomain.Cart;
using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Drafts;

namespace SampleCart.StyleB.Cart;

// 只測 ISvcCart.RemoveItem 的一個檔案。
public partial class TestCart{
	/// <summary>註冊 RemoveItem 的用例。</summary>
	/// <param name="Node">本測試類的節點。</param>
	static void RegisterRemoveItem(ITestNode Node){
		var G = Node.Group<TestCart>(typeof(ISvcCart), nameof(ISvcCart.RemoveItem));
		G.Case(nameof(RemoveExistingReturnsQty), RemoveExistingReturnsQty!);
		G.Case(nameof(RemoveMissingReturnsZero), RemoveMissingReturnsZero!);
	}

	/// <summary>移除已存在的品項，回傳被移除的數量。</summary>
	static async Task<nil> RemoveExistingReturnsQty(obj? O){
		var Svc = GetRSvc<ISvcCart>();
		await Svc.AddItem("sb-rm-hit", 4);
		var Removed = await Svc.RemoveItem("sb-rm-hit");
		Assert.IsTrue(Removed == 4, $"期望 4，實際 {Removed}");
		Assert.IsTrue(await Svc.GetItem("sb-rm-hit") is null, "移除後不應再查得到");
		return NIL;
	}

	/// <summary>移除不存在的品項，回傳 0。</summary>
	static async Task<nil> RemoveMissingReturnsZero(obj? O){
		var Svc = GetRSvc<ISvcCart>();
		var Removed = await Svc.RemoveItem("sb-rm-none");
		Assert.IsTrue(Removed == 0, $"期望 0，實際 {Removed}");
		return NIL;
	}
}
