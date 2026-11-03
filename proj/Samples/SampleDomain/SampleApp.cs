using Microsoft.Extensions.DependencyInjection;
using SampleDomain.Cart;

namespace SampleDomain;

/// <summary>
/// 五個樣本共用的應用裝配：各樣本的入口都從這裡拿到「已註冊好被測物」的容器。
/// </summary>
/// <remarks>
/// 真實工程裏這一段在應用自己的 Di 模組裏；
/// 樣本共用一份只是為了讓每個樣本的 Program.cs 只剩下「與寫法有關」的那幾行。
/// </remarks>
public static class SampleApp{
	/// <summary>建一個已註冊被測物的服務集合。</summary>
	public static IServiceCollection MkServices(){
		var S = new ServiceCollection();
		S.AddSingleton<IRepoCart, InMemoryRepoCart>();
		S.AddSingleton<ISvcCart, SvcCart>();
		return S;
	}
}
