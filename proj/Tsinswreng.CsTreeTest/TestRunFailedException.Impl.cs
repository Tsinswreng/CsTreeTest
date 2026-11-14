using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public sealed partial class TestRunFailedException{
	public partial TestRunFailedException(TestRunSummary Summary)
		: base($"Test run failed. Total={Summary.Total}, Passed={Summary.Passed}, Failed={Summary.Failed}")
	{
		this.Summary = Summary;
	}
}
