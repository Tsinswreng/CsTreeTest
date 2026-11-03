using SampleDomain.Cart;
using Tsinswreng.CsTreeTest;

namespace SampleCart.Style1.Cart;

/// <summary>測試類主檔：取被測對象、組裝各分片。</summary>
public partial class TestCart : ITester{
	/// <summary>被測對象，欄位初始化時取。</summary>
	readonly ISvcCart Svc = GetRSvc<ISvcCart>();

	/// <summary>把本測試類的用例掛到給定節點上。</summary>
	/// <param name="Node">掛載點。</param>
	public ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterAddItem(Node);
		RegisterRemoveItem(Node);
		return Node;
	}
}
