using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public abstract partial class Tester{
	public partial ITestNode RegisterTestsInto(ITestNode? Node){
		// 1: 舊入口自己帶節點來；沒帶就自建一個，行為與舊版一致。
		TestNode = Node ?? new TestNode();
		// 2: 只有第一次才填內容，避免同一測試類被掛兩次時重複掛用例。
		if(!Inited){
			Init();
			Inited = true;
		}
		// 3: 舊入口的契約是把掛好用例的節點返回。
		return TestNode;
	}
}
