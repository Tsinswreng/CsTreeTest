using SampleCart.StyleC.Cart;
using Tsinswreng.CsTreeTest.Drafts;

namespace SampleCart.StyleC;

/// <summary>
/// 樣本 C 的測試管理器：直接繼承 <c>TestMgrV2</c>，沒有「測試類」這層抽象，
/// 只有「領域分組 + 一個檔案叫甚麼名字」。
/// </summary>
/// <remarks>
/// 好處是檔數少一層（沒有 <c>_TestXxx.cs</c> 主檔），
/// 而且管理器檔案一眼看全本工程有哪些測試檔案。
/// 代價是這個工程不再有「測試類」這個可查詢維度
/// （按測試類篩選用不上）。
/// </remarks>
public class CartTestMgr : TestMgrV2{
	/// <summary>本管理器的唯一實例。</summary>
	public static CartTestMgr Inst = new();

	/// <summary>聲明本工程的測試檔案。</summary>
	protected override void SetupTree(){
		var Cart = AddBranch("Cart");
		TestAddItem.Register(Cart);
		TestRemoveItem.Register(Cart);
	}
}
