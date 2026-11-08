using Microsoft.Extensions.DependencyInjection;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Drafts.Old011;

/// <summary>
/// 已發佈版（NuGet <c>0.1.1-alpha</c>）<c>DiEtTestMgr</c> 的仿真殼。
/// </summary>
/// <remarks>
/// 這不是庫的一部分，只是為了讓「升級前」的樣本*照原樣*編譯並跑起來，
/// 好讓遷移成本可以逐檔對照。
///
/// 舊版的三個特徵都在這幾行裏：
/// 一是管理器自己要接容器（<see cref="InitSvc"/>）；
/// 二是測試類由容器解析，所以依賴走建構子注入；
/// 三是「建樹」與「裝配依賴」同一件事，順序錯了就拿到空依賴。
/// </remarks>
public abstract class DiEtTestMgr{
	/// <summary>測試樹的根。舊版叫 <c>TestNode</c>。</summary>
	public ITestNode TestNode = new TestNode{ UniqName = "Tests" };

	/// <summary>建好的容器。測試類由它解析。</summary>
	public IServiceProvider SvcProvider = null!;

	/// <summary>
	/// 舊版入口：先建容器，再建樹。兩件事同一句裏做完。
	/// </summary>
	/// <param name="SvcColct">已裝配好的服務集合。</param>
	/// <param name="MkProvider">怎麼把集合變成容器。</param>
	/// <returns>建好的容器。</returns>
	public IServiceProvider InitSvc(
		IServiceCollection SvcColct
		,Func<IServiceCollection, IServiceProvider> MkProvider
	){
		ArgumentNullException.ThrowIfNull(SvcColct);
		ArgumentNullException.ThrowIfNull(MkProvider);
		SvcProvider = MkProvider(SvcColct);
		RegisterTestsInto(TestNode);
		return SvcProvider;
	}

	/// <summary>子類在此登記本管理器的測試類。</summary>
	/// <param name="Node">掛載點；舊版允許為空。</param>
	public abstract ITestNode RegisterTestsInto(ITestNode? Node);

	/// <summary>登記一個測試類：從容器解析實例（依賴走建構子注入）。</summary>
	/// <typeparam name="T">測試類類型；須已在容器裏註冊。</typeparam>
	public void RegisterTester<T>() where T:ITester{
		var Tester = SvcProvider.GetRequiredService<T>();
		var Node = TestNode.NewTesterChild(typeof(T));
		Node.Tester = Tester;
		Tester.RegisterTestsInto(Node);
	}
}
