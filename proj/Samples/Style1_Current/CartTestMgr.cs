using SampleCart.Style1.Cart;
using Tsinswreng.CsTreeTest;

namespace SampleCart.Style1;

/// <summary>現行寫法的測試管理器。</summary>
public class CartTestMgr : TestMgr{
	/// <summary>本管理器的唯一實例。</summary>
	public static CartTestMgr Inst = new();

	/// <summary>聲明本工程的測試類。</summary>
	protected override void SetupTree(){
		RegisterTesterIntoBranch<TestCart>("Cart");
	}
}
