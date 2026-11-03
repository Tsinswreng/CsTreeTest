using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Drafts.StyleB;

/// <summary>
/// 寫法 B：測試類是靜態的，管理器只把「類型」與「掛用例的靜態函數」接起來。
/// </summary>
/// <remarks>
/// 對比寫法 A：那邊 <c>RegisterTester&lt;TestCart&gt;()</c> 會 <c>new</c> 一個實例；
/// 這邊一個物件都不建，只是靜態調用。
/// 調用方寫法幾乎沒變，變的是測試類檔案裏的 <c>public</c> → <c>public static</c>。
/// </remarks>
public abstract class TestMgrB : TestMgrV2{
	/// <summary>登記一個測試類，掛在根下。</summary>
	/// <typeparam name="T">測試類類型；須實作 <see cref="ITesterStatic"/>。</typeparam>
	/// <param name="NodeName">測試類節點的節點名；為空時取測試類短名。</param>
	public void RegisterTester<T>(str? NodeName = null) where T:ITesterStatic{
		var Node = MkTesterNode(typeof(T), NodeName, null);
		T.RegisterTestsInto(Node);
	}

	/// <summary>登記一個測試類，掛到領域分組下（分組不存在就現建）。</summary>
	/// <typeparam name="T">測試類類型；須實作 <see cref="ITesterStatic"/>。</typeparam>
	/// <param name="BranchName">領域分組名。</param>
	/// <param name="NodeName">測試類節點的節點名；為空時取測試類短名。</param>
	public void RegisterTesterIntoBranch<T>(str BranchName, str? NodeName = null)
		where T:ITesterStatic
	{
		ArgumentNullException.ThrowIfNull(BranchName);
		var Node = MkTesterNode(typeof(T), NodeName, BranchName);
		T.RegisterTestsInto(Node);
	}
}
