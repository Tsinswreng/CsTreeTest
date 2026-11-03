using Microsoft.Extensions.DependencyInjection;

namespace SampleDomain;

/// <summary>
/// 全域依賴來源：測試類（或用例）用它直接取被測對象，不走建構子注入。
/// </summary>
/// <remarks>
/// 真實工程裏這一份是*各測試工程自備*的，內容就是下面這兩段（約 6 行）；
/// 樣本共用一份只是為了讓五個樣本不要各抄一遍。
///
/// 配合各樣本的 <c>GlobalUsing.cs</c>：
/// <c>global using static SampleDomain.Di;</c>
/// 測試檔裏可以直接寫 <c>GetRSvc&lt;ISvcCart&gt;()</c>。
/// </remarks>
public static class Di{
	/// <summary>根源服務提供器。入口在 <c>BuildServiceProvider()</c> 之後賦值。</summary>
	public static IServiceProvider SvcProvider{
		get{ return field ?? throw new InvalidOperationException("Di.SvcProvider 尚未就緒。"); }
		set{ field = value ?? throw new ArgumentNullException(nameof(value)); }
	}

	/// <summary>取一個服務實例。</summary>
	/// <typeparam name="T">要取的服務類型。</typeparam>
	public static T GetRSvc<T>() where T:class{
		return SvcProvider.GetRequiredService<T>();
	}
}
