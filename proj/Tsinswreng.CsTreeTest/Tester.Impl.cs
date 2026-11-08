using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public abstract partial class Tester{
	public partial IResAddTestCase AddTestCase(str UniqName, FnTest FnTest){
		// 1: 造用例。用例名在節點上，不必另存一份。
		var testCase = new TestCase{
			FnTest = FnTest,
		};
		// 2: 掛成子節點；父子兩邊的引用都要補。
		var newNode = new TestNode{
			UniqName = UniqName,
			Data = testCase,
			Parents = [TestNode],
		};
		TestNode.Children.Add(newNode);
		// 3: 把新節點交回調用方，方便它接着設旗標或掛子節點。
		return new ResAddTestCase{
			NewTestNode = newNode,
		};
	}

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
