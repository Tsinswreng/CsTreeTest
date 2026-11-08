using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.Assertions;
using Tsinswreng.CsTreeTest.Test.Domains.Executor;
using Tsinswreng.CsTreeTest.Test.Domains.NodeTree;
using Tsinswreng.CsTreeTest.Test.Domains.Register;

namespace Tsinswreng.CsTreeTest.Test;

[Doc($$"""
#Sum[本測試工程的測試樹。]

#Descr[
按領域分組掛上各測試類，
每個領域一個分組節點，
需要單跑某個領域時，
以該分組節點為篩選範圍即可。

本工程的測試類仍用舊寫法 `{{nameof(ITester.RegisterTestsInto)}}`，
故由本類代為掛載；新寫法請直接在 `{{nameof(Tester.Init)}}` 裏調 `{{nameof(ITesterExtn.AddTester)}}`。
]
""")]
public class CsTreeTestTestMgr : Tester{
	[Doc($$"""
#Sum[全局唯一實例。]
""")]
	public static CsTreeTestTestMgr Inst = new();

	[Doc($$"""
#Sum[把各領域的測試類掛到本測試樹上。]
""")]
	public override void Init(){
		// 1: 根節點的名字固定為 Tests，方便失敗路徑可讀。
		TestNode.UniqName = "Tests";
		// 2: 一個領域一個分組節點，逐個掛上舊寫法的測試類。
		MountInBranch<TestAssert>("Assertions");
		MountInBranch<TestNodeTree>("NodeTree");
		MountInBranch<TestRegister>("Register");
		MountInBranch<TestExecutor>("Executor");
	}

	[Doc($$"""
#Sum[在指定分組下掛一個舊寫法的測試類。]
#TPrm[{{nameof(TTester)}}][測試類類型，須有無參構造子]
#Prm[{{nameof(BranchName)}}][分組名，同名分組共用一個節點]
""")]
	private void MountInBranch<TTester>(str BranchName) where TTester: ITester, new(){
		// 1: 取或建分組節點。
		var branch = TestNode.Children.FirstOrDefault(x => x.UniqName == BranchName)
			?? TestNode.NewChild(BranchName);
		// 2: 建測試類節點，再讓舊寫法的測試類往裏掛用例。
		var testerNode = branch.NewTesterChild(typeof(TTester), typeof(TTester).Name);
		new TTester().RegisterTestsInto(testerNode);
	}
}
