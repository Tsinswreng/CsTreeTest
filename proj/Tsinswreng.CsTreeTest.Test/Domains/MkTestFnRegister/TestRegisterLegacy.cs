using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.MkTestFnRegister;

// 分片聲明文件：本片測試舊 4 參數重載仍可用；實現見 TestRegisterLegacy.Impl.cs。
public partial class TestMkTestFnRegister{
	/// 用例：舊 `MkTestFnRegister(testerType, testeeTypes, testeeFnNames, prefix)` 重載行為不變。
	public partial void RegisterLegacy(ITestNode Node);
}