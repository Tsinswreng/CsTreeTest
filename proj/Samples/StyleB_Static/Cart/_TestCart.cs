using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Drafts.StyleB;

namespace SampleCart.StyleB.Cart;

/// <summary>測試類主檔：只有一個靜態入口，把各分片的註冊函數串起來。</summary>
/// <remarks>
/// 與現行寫法的差別在標記：
/// 從 <c>: ITester</c>（要有實例）變成 <c>: ITesterStatic</c>（只有靜態函數），
/// 所以 <c>RegisterTestsInto</c> 也加 <c>static</c>。
/// 這個類從頭到尾不會被 new。
/// </remarks>
public partial class TestCart : ITesterStatic{
	/// <summary>把本測試類的用例掛到給定節點上。</summary>
	/// <param name="Node">本測試類的節點。</param>
	public static ITestNode RegisterTestsInto(ITestNode Node){
		RegisterAddItem(Node);
		RegisterRemoveItem(Node);
		return Node;
	}
}
