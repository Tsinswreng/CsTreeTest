namespace SampleDomain.Cart;

/// <summary>一行購物車明細。</summary>
public class CartLine{
	/// <summary>品項編號。</summary>
	public str Sku{ get; set; } = "";

	/// <summary>數量。</summary>
	public i32 Qty{ get; set; }
}

/// <summary>被測：購物車倉儲。業務服務依賴它，所以它也是 DI 存在的理由。</summary>
public interface IRepoCart{
	/// <summary>取一行；沒有則為空。</summary>
	Task<CartLine?> Get(str Sku);

	/// <summary>寫一行（有則覆蓋）。</summary>
	Task Put(CartLine Line);

	/// <summary>刪一行；回傳是否真的刪掉了。</summary>
	Task<bool> Del(str Sku);
}

/// <summary>被測：購物車倉儲的內存實現。</summary>
/// <remarks>
/// 用併發字典：框架默認並行跑用例，而這是單例，會被多條用例同時碰。
/// 這不是樣本寫法的重點，但照真實工程的做法寫，免得樣本自己是「靠運氣過」的。
/// </remarks>
public sealed class InMemoryRepoCart : IRepoCart{
	private readonly System.Collections.Concurrent.ConcurrentDictionary<str, CartLine> _Map = [];


	public Task<CartLine?> Get(str Sku){
		return Task.FromResult(_Map.TryGetValue(Sku, out var Line) ? Line : null);
	}

	public Task Put(CartLine Line){
		_Map[Line.Sku] = Line;
		return Task.CompletedTask;
	}

	public Task<bool> Del(str Sku){
		return Task.FromResult(_Map.TryRemove(Sku, out _));
	}
}

/// <summary>被測：購物車業務服務。</summary>
public interface ISvcCart{
	/// <summary>加入 N 件；已存在則累加。</summary>
	Task AddItem(str Sku, i32 Qty);

	/// <summary>移除一個品項，回傳被移除的數量；本來就沒有則回傳 0。</summary>
	Task<i32> RemoveItem(str Sku);

	/// <summary>取一個品項的內容；沒有則為空。</summary>
	Task<CartLine?> GetItem(str Sku);
}

/// <summary>被測：購物車業務服務的實現。它自己依賴倉儲。</summary>
public sealed class SvcCart(IRepoCart Repo) : ISvcCart{
	public async Task AddItem(str Sku, i32 Qty){
		if(Qty <= 0){
			throw new ArgumentOutOfRangeException(nameof(Qty), "數量必須為正。");
		}
		var Line = await Repo.Get(Sku);
		if(Line is null){
			await Repo.Put(new CartLine{ Sku = Sku, Qty = Qty });
			return;
		}
		Line.Qty += Qty;
		await Repo.Put(Line);
	}

	public async Task<i32> RemoveItem(str Sku){
		var Line = await Repo.Get(Sku);
		if(Line is null){
			return 0;
		}
		await Repo.Del(Sku);
		return Line.Qty;
	}

	public Task<CartLine?> GetItem(str Sku){
		return Repo.Get(Sku);
	}
}
