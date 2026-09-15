using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.MkTestFnRegister;

// 分片聲明文件：本片測試泛型重載的 metadata 一次性生效；實現見 TestRegisterGenericMeta.Impl.cs。
public partial class TestMkTestFnRegister{
	/// 用例：泛型重載推斷 tester type，metadata 走參數一次傳入、不靠註冊後改屬性。
	public partial void RegisterGenericMeta(ITestNode Node);
}