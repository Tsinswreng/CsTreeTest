using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Executor;

// 實現文件：只放函數實現。
public partial class TestExecutor{
	private sealed partial class ConcurrencyProbe{
		public partial void Enter(){
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

		public partial void Exit(){
			Interlocked.Decrement(ref _Cur);
		}

		public partial async Task<obj?> Run(i32 DelayMs){
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