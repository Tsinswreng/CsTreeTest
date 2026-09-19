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

/// 本測試工程的測試管理器。
///
/// 按領域分組聲明要註冊的測試類，
/// 每個領域一個分組節點，
/// 由此演示分組寫法：
/// 需要單跑某個領域時，
/// 選器以該分組節點為範圍即可。
public class CsTreeTestTestMgr : TestMgr{
	/// 全局唯一實例。
	public static CsTreeTestTestMgr Inst = new();

	protected override void SetupTree(){
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