using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 單檔：ResAddTestCase 純數據類型；無函數體可分離。
[Doc($$"""
{{nameof(IResAddTestCase)}} 的默認實現。
""")]
public class ResAddTestCase : IResAddTestCase{
	[Doc($$"""
	#See[{{nameof(IResAddTestCase.NewTestNode)}}]
	""")]
	public ITestNode NewTestNode{get;set;} = null!;
}
