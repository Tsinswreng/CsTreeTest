using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;
using Tsinswreng.CsTreeTest.Test.Domains.Assertions;
using Tsinswreng.CsTreeTest.Test.Domains.Executor;
using Tsinswreng.CsTreeTest.Test.Domains.Filter;
using Tsinswreng.CsTreeTest.Test.Domains.Index;
using Tsinswreng.CsTreeTest.Test.Domains.Mgr;
using Tsinswreng.CsTreeTest.Test.Domains.NodeTree;
using Tsinswreng.CsTreeTest.Test.Domains.Register;
using Tsinswreng.CsTreeTest.Test.Domains.Selection;

namespace Tsinswreng.CsTreeTest.Test;

[Doc($$"""
#Sum[本測試工程的測試管理器。]

#Descr[
按領域分組聲明要註冊的測試類，
每個領域一個分組節點，
由此演示分組寫法：
需要單跑某個領域時，
以該分組節點為篩選範圍即可。

*本檔不拆聲明與實現*：
`SetupTree` 是對基類虛方法的重寫，
聲明由基類給出（與「接口+實現類」同理），
所以按規範不必要再拆 partial。
]
""")]
public class CsTreeTestTestMgr : TestMgr{
	[Doc($$"""
#Sum[全局唯一實例。]
""")]
	public static CsTreeTestTestMgr Inst = new();

	[Doc($$"""
#Sum[聲明本工程要註冊的測試類。]
#Descr[
只聲明，不實例化；
實例化發生在 `{{nameof(TestMgr.Init)}}()`。
]
""")]
	protected override void SetupTree(){
		// 一個領域一個分組節點。
		RegisterTesterIntoBranch<TestAssert>("Assertions");
		RegisterTesterIntoBranch<TestNodeTree>("NodeTree");
		RegisterTesterIntoBranch<TestIndexCase>("Index");
		RegisterTesterIntoBranch<TestRegister>("Register");
		RegisterTesterIntoBranch<TestMgrCase>("Mgr");
		RegisterTesterIntoBranch<TestExecutor>("Executor");
		RegisterTesterIntoBranch<TestFilterCase>("Filter");
		RegisterTesterIntoBranch<TestSelectionCase>("Selection");
	}
}