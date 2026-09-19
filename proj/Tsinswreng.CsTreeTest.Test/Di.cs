using Microsoft.Extensions.DependencyInjection;

namespace Tsinswreng.CsTreeTest.Test;

/// <summary>
/// 本測試工程的全局依賴來源。
///
/// 與真實測試工程（例如 Tsinswreng.CsSql.Test）完全同一個寫法：
/// 入口組裝容器後賦值，
/// 所有測試類通過 <see cref="GetRSvc{T}"/> 直接取依賴，
/// 不再使用建構子注入。
/// </summary>
public static class Di{
	/// 根源服務提供器。
	public static IServiceProvider SvcProvider{
		get{ return field ?? throw new InvalidOperationException("Di.SvcProvider has not been initialized."); }
		set{ field = value ?? throw new ArgumentNullException(nameof(value)); }
	}

	/// 從根源容器解析服務，
	/// 供測試類的欄位初始化直接取依賴。
	public static T GetRSvc<T>() where T:class{
		return SvcProvider.GetRequiredService<T>();
	}
}