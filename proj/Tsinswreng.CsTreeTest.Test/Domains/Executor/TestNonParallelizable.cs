using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Executor;

// 分片聲明文件：本片測試 ITestNode.NonParallelizable；實現見 TestNonParallelizable.Impl.cs。
[Doc($$"""
#Sum[本檔只測 ITestNode.NonParallelizable：]
#Descr[
為真時該節點及其全部遞歸子節點不參與並行，
且該聲明向下繼承。
]
""")]
public partial class TestExecutor{
	[Doc($$"""
#Sum[把 NonParallelizable 的用例掛到節點上。]
""")]
	public partial void RegisterNonParallelizable(ITestNode Node);

	[Doc($$"""
#Sum[NonParallelizable 讓子樹不與外部並行。]
""")]
	public partial Task<nil> NonParallelizableKeepsSubtreeOutOfParallel(obj? O);

	[Doc($$"""
#Sum[NonParallelizable 向下繼承到更深層。]
""")]
	public partial Task<nil> NonParallelizableIsInheritedDownwards(obj? O);
}