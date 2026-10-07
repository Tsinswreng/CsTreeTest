using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest.Test;

[Doc($$"""
#Sum[測試要用的被測服務。]

#Descr[
本工程測的是框架自身的機制，
不需要真實業務服務，
因此用一個最小的服務替身；
它的作用是讓「測試類向全局依賴來源取被測對象」這條路
被真真切切地走一遍——
測試類的欄位初始化會通過真的微軟依賴注入解析到本實現。

*本檔不拆聲明與實現*：
接口（`IFakeSvc`）已經承擔了聲明，
「接口 + 實現類」模式下不再用 partial 拆聲明與實現。
]
""")]
public interface IFakeSvc{
	[Doc($$"""
#Sum[原樣返回輸入。]
""")]
	int Echo(int X);
}

[Doc($$"""
#Sum[被測服務的默認實現。]
""")]
public sealed class FakeSvc : IFakeSvc{
	[Doc($$"""
#Sum[原樣返回輸入。]
""")]
	public int Echo(int X) => X;
}