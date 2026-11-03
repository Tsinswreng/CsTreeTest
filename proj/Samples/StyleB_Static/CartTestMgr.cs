using SampleCart.StyleB.Cart;
using Tsinswreng.CsTreeTest.Drafts.StyleB;

namespace SampleCart.StyleB;

/// <summary>
/// 樣本 B 的測試管理器：與現行寫法同形（<c>RegisterTesterIntoBranch&lt;T&gt;</c>），
/// 但 <c>T</c> 是靜態測試類，管理器不會 new 它。
/// </summary>
public class CartTestMgr : TestMgrB{
	/// <summary>本管理器的唯一實例。</summary>
	public static CartTestMgr Inst = new();

	/// <summary>聲明本工程的測試類。</summary>
	protected override void SetupTree(){
		RegisterTesterIntoBranch<TestCart>("Cart");
	}
}
