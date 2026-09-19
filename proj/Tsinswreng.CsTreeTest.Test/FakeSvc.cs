using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test;

/// 測試要用的被測服務。
///
/// 本工程測的是框架自身的機制，
/// 不需要真實業務服務，
/// 因此用一個最小的服務替身；
/// 它的作用是讓「測試類向全局依賴來源取被測對象」這條路
/// 被真真切切地走一遍——
/// 測試類的欄位初始化會通過真的微軟依賴注入解析到本實現。
public interface IFakeSvc{
	/// 原樣返回輸入。
	int Echo(int X);
}

/// 被測服務的默認實現。
public sealed class FakeSvc : IFakeSvc{
	public int Echo(int X) => X;
}