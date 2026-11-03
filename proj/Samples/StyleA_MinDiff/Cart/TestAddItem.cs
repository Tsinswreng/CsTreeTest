using SampleDomain;
using SampleDomain.Cart;
using Tsinswreng.CsTreeTest;

namespace SampleCart.StyleA.Cart;

// 只測 ISvcCart.AddItem 的一個檔案。
public partial class TestCart{
	/// <summary>註冊 AddItem 的用例。</summary>
	/// <param name="Node">本測試類的節點。</param>
	public void RegisterAddItem(ITestNode Node){
		// 註冊這一段與現行寫法完全一樣。
		var R = Node.MkRegister<TestCart>(
			typeof(ISvcCart)
			,nameof(ISvcCart.AddItem)
			,"AddItem:"
		).Register;
		var T = Assert.IsTrue;

		R("加入新條目", async (O) => {
			// 差別在這一行：用例自己取依賴。
			var Svc = GetRSvc<ISvcCart>();
			await Svc.AddItem("sa-add-new", 3);
			var Line = await Svc.GetItem("sa-add-new");
			T(Line?.Qty == 3, "期望數量 3");
			return NIL;
		});

		R("再加同品項會累加", async (O) => {
			var Svc = GetRSvc<ISvcCart>();
			await Svc.AddItem("sa-add-acc", 2);
			await Svc.AddItem("sa-add-acc", 5);
			var Line = await Svc.GetItem("sa-add-acc");
			T(Line?.Qty == 7, "期望數量 7");
			return NIL;
		});

		R("數量非正會拋", async (O) => {
			var Svc = GetRSvc<ISvcCart>();
			await Chk.Throws<ArgumentOutOfRangeException>(
				() => Svc.AddItem("sa-add-bad", 0)
			);
			return NIL;
		});
	}
}
