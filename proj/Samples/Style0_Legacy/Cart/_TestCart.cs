using SampleDomain.Cart;
using Tsinswreng.CsTreeTest;

namespace SampleCart.Style0.Cart;

/// <summary>
/// 測試類主檔（舊版寫法）：依賴走建構子注入，用例分散在 TestAddItem.cs 等檔案。
/// </summary>
public partial class TestCart : ITester{
	/// <summary>被測對象，由容器注入。</summary>
	readonly ISvcCart Svc;

	/// <summary>建構子注入：依賴由此進來，所以測試類自己也得在容器裏註冊。</summary>
	/// <param name="Svc">被測對象。</param>
	public TestCart(ISvcCart Svc){
		this.Svc = Svc;
	}

	/// <summary>把本測試類的用例掛到給定節點上。</summary>
	/// <param name="Node">掛載點。</param>
	public ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterAddItem(Node);
		RegisterRemoveItem(Node);
		return Node;
	}
}
