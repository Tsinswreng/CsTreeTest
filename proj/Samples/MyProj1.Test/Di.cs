using Microsoft.Extensions.DependencyInjection;
using Tsinswreng.CsCore;

namespace MyProj1.Test;

[Doc("""
#Sum[本測試工程的全局依賴來源。]

#Descr[
*各測試工程自備一份*，
入口在組裝容器後賦值，
所有測試類通過 `GetRSvc<T>()` 直接取依賴，
不使用建構子注入。

配合 `GlobalUsing.cs` 的
`global using static MyProj1.Test.Di;`，
測試類裏可以直接寫 `GetRSvc<ICalculator>()`，
不必寫 `Di.` 前綴。

*測試框架不參與這件事*：
它只負責把測試類 `new` 出來，
依賴由測試類自己向本類取。
所以框架不引用、
也不需要知道任何依賴注入相關的類型。
]
""")]
public static class Di{
	[Doc("""
#Sum[根源服務提供器。]

#Descr[
入口在 `BuildServiceProvider()` 之後賦值。
*必須在測試管理器 `Init()` 之前*賦值，
否則測試類的欄位初始化取依賴時會抛異常。

未賦值就取用會抛 `InvalidOperationException`，
避免拿到 `null` 之後在別處產生更難查的錯誤。
]
""")]
	public static IServiceProvider SvcProvider{
		get{ return field ?? throw new InvalidOperationException("Di.SvcProvider has not been initialized."); }
		set{ field = value ?? throw new ArgumentNullException(nameof(value)); }
	}

	[Doc("""
#Sum[從根源容器解析服務，供測試類的欄位初始化直接取依賴。]

#TParams([要取的服務類型])
#Rtn[解析出來的服務實例；容器裏沒有該服務時由容器抛異常]
""")]
	public static T GetRSvc<T>() where T:class{
		return SvcProvider.GetRequiredService<T>();
	}
}