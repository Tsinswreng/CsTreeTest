namespace Tsinswreng.CsTreeTest;

// 聲明文件：只寫函數聲明（partial method）與 Doc；實現見 Assert.Impl.cs。
public partial class Assert{
	/// 斷言成立；失敗拋帶 ErrMsg 的異常（目前唯一斷言 API）。
	public static partial nil IsTrue(bool Cond, str? ErrMsg = null);
}
