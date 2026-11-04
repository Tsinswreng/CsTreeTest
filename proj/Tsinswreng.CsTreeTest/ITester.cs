using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

[Doc($$"""
測試類要實現的接口
""")]
public interface ITester{
	[Doc($$"""
#Sum[把自己要註冊的用例掛到 `Node` 上，再把 `Node` 返回。]
#Descr[
`Node` 為空時自行新建一個節點並返回，
這樣調用方可以脫離測試樹單獨跑本測試類。
實現本方法時只負責掛用例，
不要在這裏跑用例。
]
#Params([[框架給的掛載點；測試管理器一定傳非空值，
	只在單獨調用本方法時才可能為空]])
#Rtn[掛好用例的那個節點，通常就是傳進來的 `Node`]
""")]
	ITestNode RegisterTestsInto(ITestNode? Node);
}
