using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 聲明文件：只寫函數聲明（partial method）與 Doc；實現見 _TestMgr.Impl.cs。
[Doc($$"""
#Sum[測測試管理器：分組、收編子管理器、初始化，以及按名/按類型/按被測函數查節點。]
#Descr[
本類是主檔：
只負責把各被測函數的用例檔串起來，
自己不寫用例。
]
""")]
public partial class TestMgrCase : ITester{
	[Doc($$"""
#Sum[把各分片的用例逐片掛到 `Node` 上，再把 `Node` 返回。]
#Descr[
`Node` 為空時自行新建一個節點，
這樣調用方可以脫離測試樹單獨跑本測試類。
各分片的調用順序即註冊順序。
]
#Params([[框架給的掛載點；可為空]])
#Rtn[掛好用例的那個節點]
""")]
	public partial ITestNode RegisterTestsInto(ITestNode? Node);
}