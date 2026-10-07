using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Executor;

// 分片聲明文件：本片測試 ITestNode.IsParallelRecursive；實現見 TestIsParallelRecursive.Impl.cs。
[Doc($$"""
#Sum[本檔只測 ITestNode.IsParallelRecursive：]
#Descr[
為真時該節點及其全部遞歸子節點不參與並行，
且該聲明向下繼承。
]
""")]
public partial class TestExecutor{
	[Doc($$"""
#Sum[把 IsParallelRecursive 的用例掛到節點上。]
""")]
	public partial void RegisterIsParallelRecursive(ITestNode Node);

	[Doc($$"""
#Sum[IsParallelRecursive 讓子樹不與外部並行。]
""")]
	public partial Task<nil> IsParallelRecursiveKeepsSubtreeOutOfParallel(obj? O);

	[Doc($$"""
#Sum[IsParallelRecursive 向下繼承到更深層。]
""")]
	public partial Task<nil> IsParallelRecursiveIsInheritedDownwards(obj? O);
}