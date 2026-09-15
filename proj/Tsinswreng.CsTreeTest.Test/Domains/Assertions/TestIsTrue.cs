using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Assertions;

// 分片聲明文件：本片測試 IsTrue 通過/失敗行為；實現見 TestIsTrue.Impl.cs。
public partial class TestAssert{
	/// 用例：IsTrue(true) 不拋；IsTrue(false) 拋異常且消息可自定義。
	public partial void RegisterIsTrue(ITestNode Node);
}