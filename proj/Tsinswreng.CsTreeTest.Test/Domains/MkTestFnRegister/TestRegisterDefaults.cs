using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.MkTestFnRegister;

// 分片聲明文件：本片測試不傳 metadata 時的默認值；實現見 TestRegisterDefaults.Impl.cs。
public partial class TestMkTestFnRegister{
	/// 用例：泛型重載不傳 metadata 時，TesteeTypes/TesteeFnNames 為空、無前綴。
	public partial void RegisterDefaults(ITestNode Node);
}