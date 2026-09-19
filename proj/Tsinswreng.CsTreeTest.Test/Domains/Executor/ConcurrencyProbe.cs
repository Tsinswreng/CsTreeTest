using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Executor;

public partial class TestExecutor{
	/// 並發探針。
	///
	/// 記下「同時在跑的用例數」的歷史最大值。
	/// 這樣測並行就不必靠「跑了多久」去猜：
	/// 最大值為 1 就是不並行，
	/// 大於 1 就是確實並行了。
	private sealed class ConcurrencyProbe{
		private i32 _Cur;
		private i32 _Max;

		/// 進入一條用例。
		public void Enter(){
			var Cur = Interlocked.Increment(ref _Cur);
			// 用比較交換更新最大值，
			// 免得兩條用例同時寫同一個最大值。
			i32 Old;
			do{
				Old = Volatile.Read(ref _Max);
				if(Cur <= Old){
					break;
				}
			}while(Interlocked.CompareExchange(ref _Max, Cur, Old) != Old);
		}

		/// 離開一條用例。
		public void Exit(){
			Interlocked.Decrement(ref _Cur);
		}

		/// 歷史最大同時執行數。
		public i32 Max => Volatile.Read(ref _Max);

		/// 給用例用的函數體：
		/// 進來記數、可選延時、出去減數。
		public async Task<obj?> Run(i32 DelayMs = 40){
			Enter();
			try{
				await Task.Delay(DelayMs);
			}finally{
				Exit();
			}
			return NIL;
		}
	}
}