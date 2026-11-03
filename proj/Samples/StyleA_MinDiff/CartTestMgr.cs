using SampleCart.StyleA.Cart;
using Tsinswreng.CsTreeTest.Drafts.StyleA;

namespace SampleCart.StyleA;

/// <summary>
/// 樣本 A 的測試管理器。與現行寫法逐字相同，只換了基底類別
/// （<c>TestMgr</c> → <c>TestMgrA</c>，差別是後者沒有 <c>Init()</c>）。
/// </summary>
public class CartTestMgr : TestMgrA{
	/// <summary>本管理器的唯一實例。</summary>
	public static CartTestMgr Inst = new();

	/// <summary>聲明本工程的測試類。</summary>
	protected override void SetupTree(){
		RegisterTesterIntoBranch<TestCart>("Cart");
	}
}
