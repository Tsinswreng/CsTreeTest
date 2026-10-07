using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Filter;

// 聲明文件：只寫函數聲明（partial method）與 Doc；實現見 _TestFilter.Impl.cs。
[Doc($$"""
#Sum[測篩選器 TestFilter 的各個條件、以及按範圍篩選。]

#Descr[
選對象是本工程真正初始化過的那個管理器，
篩選只讀索引，不影響並行執行。

本類是主檔：
只負責把各被測函數的用例檔串起來，
自己不寫用例。
]
""")]
public partial class TestFilterCase : ITester{
	[Doc($$"""
#Sum[把本測試類各被測函數的用例掛到節點上。]
""")]
	public partial ITestNode RegisterTestsInto(ITestNode? Node);
}