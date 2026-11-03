using Tsinswreng.CsCore;

//using Tsinswreng.CsU128Id;

namespace Tsinswreng.CsTreeTest;

// 單檔：TestNode 純數據類型；無函數體可分離。
[Doc($$"""
#Sum[測試樹節點的默認實現：純數據，無額外行爲。]
""")]
public class TestNode : ITestNode{
	[Doc(@$"#Sum[節點唯一名；本實現默認取 ULID。]
#See[{nameof(ITestNode.UniqName)}]")]
	public str UniqName{get;set;} = U128Id.NewUlid().ToLow64Base();

	[Doc(@$"#Sum[該節點攜帶的用例；非葉節點為空。]
#See[{nameof(ITestNode.Data)}]")]
	public ITestCase? Data{get;set;}

	[Doc(@$"#Sum[子節點，順序即聲明順序。]
#See[{nameof(ITestNode.Children)}]")]
	public IList<ITestNode> Children{get;set;} = [];

	[Doc(@$"#Sum[父節點；正常用法下只有一個。]
#See[{nameof(ITestNode.Parents)}]")]
	public IList<ITestNode> Parents{get;set;} = [];

	[Doc(@$"#Sum[直接子節點是否依序執行，不遞歸；默認 false。]
#See[{nameof(ITestNode.Ordered)}]")]
	public bool Ordered{get;set;} = false;

	[Doc(@$"#Sum[本子樹是否不參與並行，向下繼承；默認 false。]
#See[{nameof(ITestNode.IsParallelRecursive)}]")]
	public bool IsParallelRecursive{get;set;} = false;
}
