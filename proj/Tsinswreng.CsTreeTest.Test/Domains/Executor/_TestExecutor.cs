using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Executor;

// 聲明文件：只寫函數聲明（partial method）與 Doc；實現見 _TestExecutor.Impl.cs。
[Doc($$"""
#Sum[測執行器。]
#Descr[
本領域的用例彼此獨立，
但為了結果次序穩定，
把本節點設為依序執行，
順便演示 Ordered 屬性的用法。
本類是主檔：
只負責把各被測函數的用例檔串起來，
自己不寫用例。
]
""")]
public partial class TestExecutor : ITester{
	[Doc($$"""
#Sum[把本領域各被測函數的用例掛到節點上。]
#Descr[
本類是主檔，自己不寫用例：
只按被測函數逐片調用各分片的註冊方法，
+ `Run` 的用例在 `TestRun.cs`；
+ `{{nameof(ITestNode.Ordered)}}` 的用例在 `TestOrdered.cs`；
+ `{{nameof(ITestNode.IsParallelRecursive)}}` 的用例在 `TestIsParallelRecursive.cs`。
]
#Params([[要掛用例的節點；傳空則自行新建一個]])
#Rtn[掛好用例的節點]
""")]
	public partial ITestNode RegisterTestsInto(ITestNode? Node);
}