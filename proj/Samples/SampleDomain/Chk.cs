namespace SampleDomain;

/// <summary>
/// 樣本共用的補充斷言。
/// </summary>
/// <remarks>
/// 框架只提供 <c>Assert.IsTrue</c>（這是既定設計：領域斷言由使用方自己寫），
/// 所以「期望拋出某種異常」這種常用斷言在每個工程裏都得自己寫一條。
/// 抽在這裏是為了讓五個樣本不必各抄一遍，不是要把它塞回框架。
/// </remarks>
public static class Chk{
	/// <summary>斷言這段動作會拋出指定類型的異常。</summary>
	/// <typeparam name="TErr">期望的異常類型。</typeparam>
	/// <param name="Fn">要執行的動作。</param>
	/// <returns>非同步任務。</returns>
	public static async Task Throws<TErr>(Func<Task> Fn) where TErr:Exception{
		try{
			await Fn();
		}
		catch(TErr){
			return;
		}
		throw new Exception($"期望拋出 {typeof(TErr).Name}，但沒有拋。");
	}
}
