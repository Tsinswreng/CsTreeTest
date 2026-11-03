using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public sealed partial class TestRunSummary{
	public partial IList<TestCaseRunResult> FailedResults(){
		return Results.Where(x=>!x.IsPassed).ToList();
	}
}
