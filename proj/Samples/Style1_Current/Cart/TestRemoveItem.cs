using SampleDomain.Cart;
using Tsinswreng.CsTreeTest;

namespace SampleCart.Style1.Cart;

// 只測 ISvcCart.RemoveItem 的一個檔案。
public partial class TestCart{
	/// <summary>註冊 RemoveItem 的用例。</summary>
	/// <param name="Node">本測試類的節點。</param>
	public void RegisterRemoveItem(ITestNode Node){
		var R = Node.MkRegister<TestCart>(
			typeof(ISvcCart)
			,nameof(ISvcCart.RemoveItem)
			,"RemoveItem:"
		).Register;
		var T = Assert.IsTrue;

		R("移除已存在品項回傳原數量", async (O) => {
			await Svc.AddItem("s1-rm-hit", 4);
			var Rm = await Svc.RemoveItem("s1-rm-hit");
			T(Rm == 4, $"期望 4，實際 {Rm}");
			T(await Svc.GetItem("s1-rm-hit") is null, "移除後不應再查得到");
			return NIL;
		});

		R("移除不存在品項回傳 0", async (O) => {
			var Rm = await Svc.RemoveItem("s1-rm-none");
			T(Rm == 0, $"期望 0，實際 {Rm}");
			return NIL;
		});
	}
}
