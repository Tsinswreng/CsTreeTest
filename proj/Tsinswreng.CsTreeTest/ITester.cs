using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

[Doc($$"""
測試類：把自己要跑的用例掛到給定的節點上。

測試類由 `{{nameof(TestMgr)}}` 在 `{{nameof(TestMgr.Init)}}` 時 `new` 出來（無參構造，依賴自取），
再回調 `{{nameof(RegisterTestsInto)}}`。
""")]
public interface ITester{
	[Doc($$"""
把用例掛到 `{{nameof(Node)}}` 上並返回它；Node 為空時自建一個。

只掛用例，不在這裏跑用例。

#Rtn[掛好用例的那個節點]
""")]
	ITestNode RegisterTestsInto(ITestNode? Node);
	

}

public interface ITester2{
	public ITestNode TestNode{get;set;}
	public void Init();
	public bool Inited{get;set;}
}

public abstract class Tester2:ITester2{
	public abstract void Init();
	public ITestNode TestNode{get;set;}
	
	public void AddTester(ITester2 Tester, Type? TesterType){
		if(!Tester.Inited){
			Tester.Init();
		}
		TestNode.Children.Add(Tester.TestNode);//TswgNote 記錄 TesterType 信息
	}
	
	public void AddTester<TTester>()
		where TTester: ITester2, new()
	{
		var tester = new TTester();
		AddTester(tester, typeof(TTester));
	}
	
	public void AddTestCase(str UinqName, FnTest FnTest){
		
	}
	
}
