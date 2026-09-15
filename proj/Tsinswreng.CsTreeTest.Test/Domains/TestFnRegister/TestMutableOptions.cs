using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.TestFnRegister;

// 分片聲明文件：本片測試「註冊間修改 option 影響後續 Register」；實現見 TestMutableOptions.Impl.cs。
public partial class TestTestFnRegister{
	/// 用例：改 UniqNamePrefix 後再 Register，只有新用例帶上最新前綴。
	public partial void RegisterMutableOptions(ITestNode Node);
}