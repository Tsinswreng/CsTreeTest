using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public sealed partial class TestRunFailedException{
	public partial TestRunFailedException(TestRunSummary Summary)
		: base($"測試有失敗。總數={Summary.Total}，通過={Summary.Passed}，失敗={Summary.Failed}")
	{
		this.Summary = Summary;
	}
}
