using SampleDomain;
using SampleDomain.Cart;
using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Drafts.Old011;

namespace SampleCart.Style0.Cart;

// 只測 ISvcCart.AddItem 的一個檔案。
public partial class TestCart{
	/// <summary>註冊 AddItem 的用例。</summary>
	/// <param name="Node">本測試類的節點。</param>
	public void RegisterAddItem(ITestNode Node){
		var register = Node.MkTestFnRegister(
			typeof(TestCart)
			,[typeof(ISvcCart)]
			,[nameof(ISvcCart.AddItem)]
			,"AddItem:"
		);
		var R = register.Register;
		var T = Assert.IsTrue;

		R("加入新條目", async (O) => {
			await Svc.AddItem("s0-add-new", 3);
			var Line = await Svc.GetItem("s0-add-new");
			T(Line?.Qty == 3, "期望數量 3");
			return null;
		});

		R("再加同品項會累加", async (O) => {
			await Svc.AddItem("s0-add-acc", 2);
			await Svc.AddItem("s0-add-acc", 5);
			var Line = await Svc.GetItem("s0-add-acc");
			T(Line?.Qty == 7, "期望數量 7");
			return null;
		});

		R("數量非正會拋", async (O) => {
			await Chk.Throws<ArgumentOutOfRangeException>(
				() => Svc.AddItem("s0-add-bad", 0)
			);
			return null;
		});
	}
}
