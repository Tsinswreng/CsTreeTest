using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Executor;

// 分片聲明文件：本片測試有序執行與 Arg 傳遞；實現見 TestOrderAndArg.Impl.cs。
public partial class TestTreeTestExecutor{
	/// 用例：Ordered 節點按聲明順序執行；Arg 傳遞給每個用例。
	public partial void RegisterOrderAndArg(ITestNode Node);
}