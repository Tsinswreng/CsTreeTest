using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Selection;

// 分片聲明文件：本片測試 TestSelection.Run；實現見 TestRun.Impl.cs。
[Doc($$"""
#Sum[本檔只測 TestSelection.Run：]
#Descr[
選集可直接執行，
執行結果的總數等於選中用例數。
]
""")]
public partial class TestSelectionCase{
	[Doc($$"""
#Sum[把 Run 的用例掛到節點上。]
""")]
	public partial void RegisterRun(ITestNode Node);

	[Doc($$"""
#Sum[選集可直接執行。]
""")]
	public partial Task<nil> SelectionIsRunnableDirectly(obj? O);
}