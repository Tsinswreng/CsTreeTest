using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Executor;

// 分片聲明文件：本片測試 ITestNode.Ordered；實現見 TestOrdered.Impl.cs。
[Doc($$"""
#Sum[本檔只測 ITestNode.Ordered：]
#Descr[
為真時直接子節點依序執行，
且不遞歸——
單個子節點內部仍可並行。
]
""")]
public partial class TestExecutor{
	[Doc($$"""
#Sum[把 Ordered 的用例掛到節點上。]
""")]
	public partial void RegisterOrdered(ITestNode Node);

	[Doc($$"""
#Sum[Ordered 只管直接子節點且不遞歸。]
""")]
	public partial Task<nil> OrderedAffectsOnlyDirectChildrenAndIsNotRecursive(obj? O);
}