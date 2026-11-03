using Tsinswreng.CsTreeTest;

namespace SampleCart.StyleA.Cart;

/// <summary>測試類主檔：沒有任何依賴欄位，只組裝各分片。</summary>
/// <remarks>
/// 與現行寫法的差別就這一行：
/// 現行寫法在這裏寫 <c>readonly ISvcCart Svc = GetRSvc&lt;ISvcCart&gt;();</c>，
/// 逼得「建樹」必須晚於「依賴就緒」；
/// 本寫法把取依賴推遲到每條用例執行時，於是這一層不再需要依賴。
/// </remarks>
public partial class TestCart : ITester{
	/// <summary>把本測試類的用例掛到給定節點上。</summary>
	/// <param name="Node">掛載點。</param>
	public ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterAddItem(Node);
		RegisterRemoveItem(Node);
		return Node;
	}
}
