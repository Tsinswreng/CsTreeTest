using MyProj1.Test.MyDomains.Calculator;
using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace MyProj1.Test;

[Doc($$"""
#Sum[本測試工程的測試樹。]

#Descr[
本工程只有一個測試類 `TestCalculator`，直接掛在根下。

用例要在依賴來源就緒之後才掛，
所以入口先備好依賴，再調 `{{nameof(Init)}}()`。
]
""")]
public class MyProj1TestMgr : Tester{
	[Doc("""
#Sum[本測試樹的全局唯一實例。]
""")]
	public static MyProj1TestMgr Inst = new();

	[Doc("""
#Sum[把本工程的測試類掛到根節點下。]
""")]
	public override void Init(){
		// 根節點名固定為 Tests，失敗路徑才可讀。
		TestNode.UniqName = "Tests";
		Mount<TestCalculator>(nameof(TestCalculator));
	}

	[Doc($$"""
	#Sum[掛一個舊寫法的測試類：由本類建節點，讓它往裏掛用例。]
	#TPrm[{{nameof(TTester)}}][測試類類型，須有無參構造子]
	#Prm[{{nameof(NodeName)}}][測試類節點名]
	""")]
	private void Mount<TTester>(str NodeName) where TTester: ITester, new(){
		var node = TestNode.NewTesterChild(typeof(TTester), NodeName);
		new TTester().RegisterTestsInto(node);
	}
}
