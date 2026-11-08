using SampleCart.Style1.Cart;
using Tsinswreng.CsTreeTest;

namespace SampleCart.Style1;

/// <summary>現行寫法的測試管理器。</summary>
public class CartTestMgr : Tester{
	/// <summary>本管理器的唯一實例。</summary>
	public static CartTestMgr Inst = new();

	/// <summary>聲明本工程的測試類。</summary>
	public override void Init(){
		// 根節點名固定，失敗路徑才可讀。
		TestNode.UniqName = "Tests";
		var branch = TestNode.NewChild("Cart");
		var node = branch.NewTesterChild(typeof(TestCart), nameof(TestCart));
		new TestCart().RegisterTestsInto(node);
	}
	/// <summary>掛一個舊寫法的測試類：由本類建節點，讓它往裏掛用例。</summary>
	private void Mount<TTester>(str NodeName) where TTester: ITester, new(){
		var node = TestNode.NewTesterChild(typeof(TTester), NodeName);
		new TTester().RegisterTestsInto(node);
	}
}
