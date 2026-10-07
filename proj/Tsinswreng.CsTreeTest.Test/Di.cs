using Microsoft.Extensions.DependencyInjection;
using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest.Test;

[Doc($$"""
#Sum[本測試工程的全局依賴來源。]

#Descr[
與真實測試工程（例如 Tsinswreng.CsSql.Test）完全同一個寫法：
入口組裝容器後賦值，
所有測試類通過 `GetRSvc<T>()` 直接取依賴，
不使用建構子注入。

配合 `GlobalUsing.cs` 的
`global using static Tsinswreng.CsTreeTest.Test.Di;`，
測試類裏可直接寫 `GetRSvc<IFakeSvc>()`。

*本檔不拆聲明與實現*：
它與真實調用方 `Tsinswreng.CsSql.Test/Di.cs` 逐字同構，
是測試工程的基礎設施（fixture），不屬於要被拆分的 API 表面。
]
""")]
public static class Di{
	[Doc($$"""
#Sum[根源服務提供器。]
#Descr[
入口在 `BuildServiceProvider()` 之後賦值，
必須早於測試管理器的 `{{nameof(TestMgr.Init)}}()`。
未賦值就取用會抛 `InvalidOperationException`，
避免拿到 `null` 後在別處產生更難查的錯誤。
]
""")]
	public static IServiceProvider SvcProvider{
		get{ return field ?? throw new InvalidOperationException("Di.SvcProvider has not been initialized."); }
		set{ field = value ?? throw new ArgumentNullException(nameof(value)); }
	}

	[Doc($$"""
#Sum[從根源容器解析服務，供測試類的欄位初始化直接取依賴。]
#TParams([要取的服務類型])
#Rtn[解析出來的服務實例]
""")]
	public static T GetRSvc<T>() where T:class{
		return SvcProvider.GetRequiredService<T>();
	}
}