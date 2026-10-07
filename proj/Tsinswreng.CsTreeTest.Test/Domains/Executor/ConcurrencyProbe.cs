using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Executor;

// 分片聲明文件：本片測試 並發探針 ConcurrencyProbe；實現見 ConcurrencyProbe.Impl.cs。
public partial class TestExecutor{
	[Doc($$"""
#Sum[並發探針。]
#Descr[
記下「同時在跑的用例數」的歷史最大值。
這樣測並行就不必靠「跑了多久」去猜：
最大值為 1 就是不並行，
大於 1 就是確實並行了。
]
""")]
	private sealed partial class ConcurrencyProbe{
		[Doc($$"""
#Sum[當前同時在跑的用例數。]
""")]
		private i32 _Cur;

		[Doc($$"""
#Sum[歷史最大同時執行數的存儲。]
""")]
		private i32 _Max;

		[Doc($$"""
#Sum[進入一條用例。]
""")]
		public partial void Enter();

		[Doc($$"""
#Sum[離開一條用例。]
""")]
		public partial void Exit();

		[Doc($$"""
#Sum[給用例用的函數體：進來記數、可選延時、出去減數。]
#Params([[延時毫秒數；用來把「同時在跑」的時間窗口拉長]])
#Rtn[`NIL`]
""")]
		public partial Task<obj?> Run(i32 DelayMs = 40);

		[Doc($$"""
#Sum[歷史最大同時執行數。]
""")]
		public i32 Max => Volatile.Read(ref _Max);
	}
}