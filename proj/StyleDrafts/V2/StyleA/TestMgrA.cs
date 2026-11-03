using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Drafts.StyleA;

/// <summary>
/// 寫法 A（最小改動）：測試類仍然是*有狀態的實例*、依賴仍然由測試類持有，
/// 但去掉「初始化」這一步，且依賴改在用例執行時取。
/// </summary>
/// <remarks>
/// 與庫本體 <see cref="TestMgr"/> 的差別只有兩處：
/// 一是沒有 <see cref="TestMgr.Init"/>（樹在第一次用到的時候就建好）；
/// 二是 <c>SetupTree</c> 裏的登記*當場就掛樹*，不再進待辦隊列。
///
/// 測試類仍然要求無參建構子（<c>new()</c>），
/// 所以依賴不能走建構子；本寫法的做法是用例函數體內自己 <c>GetRSvc&lt;T&gt;()</c>。
/// </remarks>
public abstract class TestMgrA : TestMgrV2{
	/// <summary>登記一個測試類，掛在根下。</summary>
	/// <typeparam name="T">測試類類型；須有無參建構子。</typeparam>
	/// <param name="NodeName">測試類節點的節點名；為空時取測試類短名。</param>
	public void RegisterTester<T>(str? NodeName = null) where T:ITester, new(){
		var Tester = new T();
		var Node = MkTesterNode(typeof(T), NodeName, null);
		Node.Tester = Tester;
		Tester.RegisterTestsInto(Node);
	}

	/// <summary>登記一個測試類，掛到領域分組下（分組不存在就現建）。</summary>
	/// <typeparam name="T">測試類類型；須有無參建構子。</typeparam>
	/// <param name="BranchName">領域分組名。</param>
	/// <param name="NodeName">測試類節點的節點名；為空時取測試類短名。</param>
	public void RegisterTesterIntoBranch<T>(str BranchName, str? NodeName = null)
		where T:ITester, new()
	{
		ArgumentNullException.ThrowIfNull(BranchName);
		var Tester = new T();
		var Node = MkTesterNode(typeof(T), NodeName, BranchName);
		Node.Tester = Tester;
		Tester.RegisterTestsInto(Node);
	}
}
