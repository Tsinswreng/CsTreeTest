using SampleCart.Style0.Cart;
using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Drafts.Old011;

namespace SampleCart.Style0;

/// <summary>
/// 舊版（0.1.1-alpha）寫法的測試管理器：繼承 <c>DiEtTestMgr</c>，
/// 自己接容器，並在 <c>RegisterTestsInto</c> 裏登記測試類。
/// </summary>
public class CartTestMgr : DiEtTestMgr{
	/// <summary>本管理器的唯一實例。</summary>
	public static CartTestMgr Inst = new();

	/// <summary>登記本工程的測試類。</summary>
	/// <param name="Node">舊版允許為空的掛載點。</param>
	public override ITestNode RegisterTestsInto(ITestNode? Node){
		Node = this.TestNode;
		this.RegisterTester<TestCart>();
		return Node;
	}
}
