using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Drafts.StyleB;

/// <summary>
/// 寫法 B 的測試類契約：測試類是一組*靜態*函數，管理器不實例化它。
/// </summary>
/// <remarks>
/// 為甚麼靜態：測試類本來就沒有「物件狀態」可言——
/// 依賴改成用例執行時取之後，一個實例欄位也沒有留下的必要。
/// 去掉實例就順帶去掉了這些約束：
/// 不需要無參建構子、不需要 <c>new()</c>、
/// 不存在「一個實例被全部用例共用」的串味問題。
///
/// 注意 C# 的靜態類（<c>static class</c>）不能實作介面，
/// 所以測試類寫成「有靜態成員的普通 partial class」。
/// </remarks>
public interface ITesterStatic{
	/// <summary>把本測試類的用例掛到給定節點上。</summary>
	/// <param name="Node">框架給的掛載點，一定非空。</param>
	/// <returns>掛好用例的那個節點。</returns>
	static abstract ITestNode RegisterTestsInto(ITestNode Node);
}
