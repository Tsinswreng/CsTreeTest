using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Selection;

// 聲明文件：只寫函數聲明（partial method）與 Doc；實現見 _TestSelection.Impl.cs。
[Doc($$"""
#Sum[測選集 TestSelection：]
#Descr[
篩選的結果、由結果建臨時樹、以及直接執行選集。
本類是主檔：
只負責把各被測函數的用例檔串起來，
自己不寫用例。
]
""")]
public partial class TestSelectionCase : ITester{
	[Doc($$"""
#Sum[把本領域各被測函數的用例掛到節點上。]
#Descr[
本類是主檔，自己不寫用例：
只按被測函數逐片調用各分片的註冊方法，
+ `{{nameof(TestFilter.Select)}}` 的用例在 `TestSelect.cs`；
+ `{{nameof(TestSelection.MkDetachedTree)}}` 的用例在 `TestMkDetachedTree.cs`；
+ `Run` 的用例在 `TestRun.cs`。
]
#Params([[要掛用例的節點；傳空則自行新建一個]])
#Rtn[掛好用例的節點]
""")]
	public partial ITestNode RegisterTestsInto(ITestNode? Node);
}