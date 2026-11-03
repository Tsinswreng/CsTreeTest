-T[2026_1021_213308][
	看skill和記憶。
	現在我想緟構一下 CsTreeTest。
	
	
	可以不兼容 已經發在Nuget上的舊版
	但是最好要做到
	從舊版升級新版 原本舊版的調用方 不用大面積更改
	只用改一小部分固定的配置
	
	具體改的方向就是:
	- 本庫不依賴DI
		- 讓用例自己調用Di.GetRSvc<>
	- 寫法要簡單
	- 裏面 樹 的結構不用丟
	- 仍能分文件寫
	先想想怎麼做。
][
````md
看完 skill 與記憶後，我先讀了本地 `Tsinswreng.CsTreeTest` 的現狀，再談方案。以下按「現有 / 我的判斷 / 可選方案 / 待你拍板」分開講。

## 一、現有事實（已讀碼確認）

本地源碼狀態：`0.2.0-alpha`（`dev1` 分支，未發包），NuGet 上是 `0.1.1-alpha`。也就是「去 DI」這一輪在本地**已經做過一半**：`DiEtTestMgr`、`InitSvc`、`MkTestFnRegister` 在 0.2.0 源碼裏**已經不存在**，改成 `TestMgr` + `SetupTree` + `Init()` + `Node.MkRegister<TTester>()`。

現在一個「一個被測函數一個文件」的標準寫法，成本是這些：

- 主檔 `_TestXxx.cs`：`public partial class TestXxx : ITester`，實例欄位 `private readonly ICalculator _C = GetRSvc<ICalculator>();`，`RegisterTestsInto(ITestNode? Node)` 裏 `Node ??= new TestNode();` 再逐行 `RegisterAdd(Node);`
- 每函數檔：`var R = Node.MkRegister<TestXxx>(typeof(ICalc), nameof(ICalc.Add), "Add:").Register;` 然後 `R("用例名", async (o) => {...; return NIL;});`
- mgr：`DiEtTestMgr`→`TestMgr`，`SetupTree` 裏 `RegisterTester<T>()`；`RegisterTester<T>` 的約束是 `where T:ITester,new()`
- 入口三步，**順序有硬約束**：先 `Di.SvcProvider = ...`，再 `Mgr.Init()`，最後跑
- 為什麼有這個順序：`Init()` 會 `new` 測試類，而測試類在**欄位初始化**時就取依賴

規模事實：工作區裏 `MkTestFnRegister` / `DiEtTestMgr` 這代寫法的調用點約 **250 處**（`Ngan.Dict`、`RimeTools`、`Avln.Dsl`、`CsRefl`、`TswgCsTpl` 等），要嘛升 `Tsinswreng.CsTreeTest` 版本，要嘛一直釘在 0.1.1。

## 二、我的判斷：複雜度的根因只有一條

「DI 在 tester 建構期取」這一件事，派生出了現在**所有**的繁複：

- 因為要先就緒 DI 才能 new tester，所以 mgr 被迫分兩段（`SetupTree` 只聲明、`Init()` 才實例化），內含 `PendingTester` 隊列、`_pendingSubMgrs` 延後收編、`_initialized` 校驗、索引整體重建；
- 因為 tester 由 mgr 無參 `new`，所以 tester 不能有 ctor 參數，依賴只能塞欄位；
- 因為 tester 是實例，所以 `ITesterNode` 還要存一份 `Tester` 實例，入口還得講究時序。

你說的「讓用例自己調 `Di.GetRSvc<>`」正好解掉這條根因：**依賴在用例執行時才取，就沒有任何東西需要在建樹前就緒**。於是 mgr 可以建構即建樹、`Init()` 這一步整個消失、tester 可以完全無狀態。所以我推薦的重構主軸不是「把 DI 挪走」（那已做過了），而是「把依賴取得期從建構期移到執行期，順勢拆掉兩段式生命週期」。

還有一個附帶收穫：拆掉 `new()` 約束後，`_TestXxx.cs` 主檔變成純靜態組裝，`Node ??= new TestNode()` 這種「為了能單獨跑」的自保寫法也不必寫了。

## 三、三個方案

三個方案共同點（都不動）：樹節點結構（`ITestNode`/`Children`/`Ordered`/`IsParallelRecursive`）、執行器與 stage 模型、索引與 `TestFilter` 篩選、`Assert`。

### 方案 A：最小改動（保留實例 tester）

只在現有 0.2.0 上做兩刀：依賴改在用例內取；拆掉 `Init()` 兩段式，mgr 建構即建樹。

- 寫法：tester 仍是 `public partial class TestXxx : ITester`，`RegisterTestsInto` 仍是實例方法；`RegisterTester<T>()` 不變（`new()` 約束保留）。
- 入口：兩步（配 DI、跑）。
- 優點：對 250 處舊代碼衝擊最小，庫內改動集中在 `TestMgr`；用例註冊行**一行都不用改**（只要我們保留 `MkTestFnRegister` 作別名）。
- 缺點：只省掉一個 `Init()` 步驟；`new()` 約束、tester 實例、`Tester` 欄位、主檔 boilerplate 全都還在。說實話，這不算「寫法變簡單」，只算「少一步」。

### 方案 B：tester 去實例化（我推薦）

新增一個委託／靜態契約，讓「測試類」從實例變成純靜態組裝點，`TestMgr` 不再 `new` 任何東西。

- 新增 `FnRegisterTestsInto` 為委託型別 `ITestNode (ITestNode Node)`？不，我把選擇權留給 `ITester` 的形態（見下），核心是：**mgr 直接拿到「把用例掛到節點」這個動作，而不是一個物件**。
- 兩種實現形態，結果一樣、遷移成本差很多：
  - B1：`ITester` 改為帶 `static abstract ITestNode RegisterTestsInto(ITestNode Node)` 的靜態契約，`RegisterTester<T>()` 的調用文字**完全不變**（`where T:ITester`，去掉 `new()`）。C# 要求靜態類不能實作介面，所以 tester 是「有靜態成員的普通 partial class」。舊 tester 檔只需 `public` → `public static` 一行。
  - B2：定義委託 `FnCasesInto = void (ITestNode Node)` 為「掛用例的動作」，mgr 寫 `RegisterTester<TestXxx>(TestXxx.RegisterTestsInto)`。更直白，但每處 mgr 多一個參數。
- 依賴：用例函數體內 `var C = Di.GetRSvc<ICalculator>();`（要共享就 tester 內 `static` 欄位，但注意它會在**第一次碰 tester** 時就觸發，等於又回到時序約束，所以默認建議用例內取）。
- `TestMgr`：刪 `SetupTree` 兩段式 → 改 `protected override void RegisterTests(ITestNode Root)`（mgr 建構時調一次），刪 `PendingTester`/`_pendingSubMgrs`/`Init`/索引重建/`_initialized`。
- 優點：無 ctor、無 `new()`、無共享實例、無時序約束；mgr 內部複雜度砍掉一大塊（`TestMgr.cs` 現在 750 行，其中大半在講這套兩段式）；入口變兩步。
- 缺點：失去「tester 實例當 fixture 共享狀態」的能力（現在其實也沒 per-case 隔離，是共享實例，屬既有的定時炸彈）；靜態契約對不熟 C# 11 的人要解釋一句。

### 方案 C：以「文件」為註冊單位（最激進）

把 tester 類與主檔整個取消：每個被測函數一個文件，文件自報「測誰的哪個函數」，mgr 直接列文件。

- 寫法：`TestAdd.RegisterAdd(ITestNode Node)` 這樣一個靜態入口；mgr 裏 `RegisterCases<ICalculator>(nameof(ICalculator.Add), TestAdd.RegisterAdd);`
- 優點：檔數少一層（沒有 `_TestXxx.cs` 主檔），mgr 一眼看全工程。
- 缺點：**按「測試類」篩選與索引會失去意義**（現在 `TestIndex.TesterType_TestNode` 是一個可查詢維度），要嘛改成按被測類型篩，要嘛引入「檔案類別」概念；mgr 會隨被測函數數量線性變長；與 skill 與既有 250 處的心智模型差距最大。

**推薦度：B（B1 為主，遷移時同時接受 B2 寫法）> A > C。** 理由：B 是唯一同時滿足你四條約束（不依賴 DI、寫法簡單、樹不丟、能分文件）又不砍掉篩選能力的方案；C 砍掉的正好是你可能要用的能力；A 簡化幅度不足以支撐一次 breaking 重構。

### 另外三個「減負選項」（與 A/B/C 正交，可獨立採用）

這三條是純新增、不改語義，專門砍每檔 boilerplate：

1. 元數據只寫一次、掛在** group 節點**上：新增 `ITestNode.Group<TTester>(Type TesteeType, str TesteeFnName, str Prefix)`，之後每條只 `G.Case(...)`。順帶讓失敗路徑變成 `Tests/Calculator/TestCalculator/Add/正數相加`（前綴機制可以退場）。
2. 用例重載去掉無用參數：新增 `Case(str Name, Func<Task> Fn)` 與 `Case(str Name, Action Fn)`。現在每條都得寫 `async (o) => { ...; return NIL; }`，多數用例根本不用 `Arg`。
3. 舊名別名：保留 `MkTestFnRegister(...)` 指向新的註冊器（同簽名、同 `.Register` 方法組），這樣 250 處用例註冊行**零改動**，把遷移成本壓成「每個 tester 檔一行 + 每個 mgr 一行 + 入口刪一行」。

## 四、舊版（NuGet 0.1.1）→ 新版的固定改動清單

若採 B1 + 減負選項 1、2、3，舊調用方要改的只有這四類，全是機械改動：

- 每個 tester 主檔：`class TestXxx : ITester` → 加 `static`；`Di` 注入的欄位改成用例內 `GetRSvc<>`（只改用到依賴的那幾條用例）
- 每個 tester 分片檔：`public void RegisterAdd(ITestNode Node)` → 加 `static`；元數據那幾行可留可換
- 每個 `*TestMgr.cs`：`DiEtTestMgr` → `TestMgr`、`RegisterTestsInto` 重寫成 `RegisterTests`、`InitSvc(...)` 整行刪掉
- 每個入口 `Program.cs`：`Mgr.Init()` 刪掉

用例名、用例體、斷言、分檔方式全部不動。

## 五、待你拍板

1. `Tsinswreng.CsTreeTest/_Tswg/2026_1021_213255/Refac.typ` 看起來是這次重構的筆記。按規矩我沒讀 —— **要我讀嗎？**（若你自己已寫好方向，我就照它做，不再猜。）
2. 依賴取得粒度：**(a) 每條用例體內自己 `GetRSvc<>`（我推薦）**、(b) tester 內 `static` 欄位共享（省字但把時序約束請回來）、(c) 兩者都允許，文檔建議 a。
3. tester 形態：**(a) B1 靜態契約（改動最小，`RegisterTester<T>()` 一字不改）**、(b) B2 委託參數（更直白）、(c) 保留實例（方案 A）。
4. 是否同意拆掉 `Init()` 兩段式生命週期？我認為必拆，這是本次簡化的主體。
5. 是否要那三個減負選項（group 節點帶元數據、無參用例重載、舊名別名）？三者互相獨立，可只取部分。
6. `ITester` 這個名字要保留嗎？若走 B1，它會從「實例契約」變成「靜態契約」，同名不同義，可能反而礙事（可改叫 `ITestCases` 之類）。

第 1 條若你說「自己看筆記」，我就把方案收斂到只剩一套再來；若你先答 2/3/4/5/6，我也可以直接出最終設計（含要改的檔案清單與分階段落地順序），你看過再動碼。
````
]


-T[2026_1021_214612][
	光聽你這樣講
	我是看不懂的。
	`E:\_code\Ngan\Tsinswreng.CsTreeTest\proj`
	你自己在這下面多建幾個程序集 展示不同寫法。
	我要從調用方的寫法入手來評估
][
````md
建好了。五個可跑的樣本程序集，外加一個只放候選寫法的草稿庫；庫本體 `proj/Tsinswreng.CsTreeTest/**` 一行未改。

## 先看這份

`proj/Samples/README.typ` —— 五個樣本的對照說明（每檔固定行數、升級要改哪幾行、已知問題）。想直接看代碼的話，按這個順序看最省力：

1. `proj/Samples/Style0_Legacy/` —— 升級前（0.1.1 形狀）
2. `proj/Samples/Style1_Current/` —— 庫本體現行（0.2.0 形狀）
3. `proj/Samples/StyleA_MinDiff/` —— 最小改動
4. `proj/Samples/StyleB_Static/` —— 我推薦的方向
5. `proj/Samples/StyleC_FileUnit/` —— 最激進

五個樣本測同一批 5 條用例、同一個被測域（`ISvcCart` 依賴 `IRepoCart`，所以「依賴從哪來」的差異才看得出來），**只有調用方寫法不同**。並排看兩個目錄，差異就是換寫法的全部代價。

## 各樣本長什麼樣

**Style0（0.1.1）**：`DiEtTestMgr` + `InitSvc` + 建構子注入 + `MkTestFnRegister`。測試類還得自己註冊進容器（`SvcColct.AddSingleton<TestCart>()`）。它跑在草稿庫的舊版仿真殼上，所以是真編譯、真跑。

**Style1（現行）**：入口三步（依賴 → `Init()` → 跑）；主檔一行 `readonly ISvcCart Svc = GetRSvc<ISvcCart>();`；每檔 6 行註冊骨架，用例是 `R("名字", async (O) => {...; return NIL;});`。

**StyleA（最小改動）**：與 Style1 只差兩處——管理器換成沒有 `Init()` 的版本（`SetupTree` 內容一字不改）、依賴改成用例執行時取（用例體多一行 `var Svc = GetRSvc<ISvcCart>();`）。入口剩兩步。這就是「只改一小部分固定配置」的實際樣子。

**StyleB（推薦方向）**：測試類改成靜態（`: ITesterStatic`），每個被測函數多一層真節點（分組）並在那一層寫一次元數據，之後每條用例只有 `G.Case(nameof(X), X!);`，用例本身是常態具名函數。不需要前綴，樹是 `Tests/Cart/TestCart/AddItem/AddNewLine`。附帶好處：分組是節點，所以能只把某一個被測函數的用例串行化。

**StyleC（檔案為單位）**：沒有測試類、沒有主檔。管理器直接列 `TestAddItem.Register(Cart);`，樹是 `Tests/Cart/AddItem/AddNewLine`。檔數最少，但失去「測試類」這個篩選維度。

## 實測結果（都跑過）

五個樣本全部 5/5 通過、退出碼 0。B 與 C 在入口裏順手印了樹路徑與篩選結果：B 按被測函數名篩出 2 條，C 按被測類型篩出 5 條——說明樹、索引、篩選都沒有被簡化掉。

跑法（這台機器必須帶那兩個參數，原因下面講）：

```
dotnet restore <樣本>/<樣本>.csproj -p:RestoreUseStaticGraphEvaluation=true --force
dotnet build <樣本>/<樣本>.csproj -m:1 --no-restore
./<樣本>/bin/Debug/net10.0/<樣本>.exe
```

## 幾個必須先跟你講清楚的坑

- 本機 `dotnet build` 對「有 ProjectReference 的新工程」會靜默失敗，表現是「0 個錯誤卻生成失败」，實際是嵌套 MSBuild 的 MSDK 解析失敗。可行組合就是上面那兩條；`dotnet run` 仍會失敗，所以直接跑 exe。這與樣本代碼無關。
- 具名用例要寫 `X!`：用例函數回 `Task<nil>`（`nil = object`，非可空），而框架的 `FnTest` 是 `Task<obj?>`，不寫就有 CS8621。這是庫本體既有形狀，不是新寫法引入的。
- 節點唯一名是**全樹**唯一，不只是同層唯一。B/C 的分組節點默認取被測函數名，所以兩個被測類型有同名函數時會撞名（`TestIndex` 直接拋）。
- 樣本 fixture 是單例、而框架默認並行，所以內存倉儲第一版就隨機炸了，已改成 `ConcurrentDictionary`。這件事本身也值得記住：默認並行 + 共享 fixture 是會咬人的。

## 我的判斷（供你否決）

- 「依賴改成用例執行時取」不是為了省字，而是它是**唯一能拆掉兩段式生命週期（`SetupTree`/`Init`）的前提**；拆掉後 `TestMgr.cs` 裏 `PendingTester`、`_pendingSubMgrs`、索引重建那一大套都可以消失。
- 舊版那 250 處 `MkTestFnRegister(...)` 的 5 行**可以零改動**，前提是新版留個同名別名。所以真正要改的只有：每個主檔的依賴欄位、每個 `*TestMgr.cs` 的基底類與 `InitSvc` 行、每個入口的 `Mgr.Init()` 行。
- 若只能選一個最終形態，我選 B：減負最多，又保住篩選維度；A 可以當它的遷移墊腳石（先讓舊工程照 A 改一步就跑起來，再逐步換成 B）。

## 要你拍板

1. 看過樣本後選 A、B、C，還是 A→B 分兩步走？
2. B 的兩個新增概念能不能接受：`ITesterStatic` 這個名字，以及「分組即節點」（樹多一層、路徑變長一級）。
3. 舊名別名（`MkTestFnRegister` / `DiEtTestMgr` / `InitSvc`）要不要留？
4. 分組節點名的唯一性怎麼約定：默認取被測函數名（可能撞）、自動加序號（路徑會變醜）、還是 `TesteeType名.函數名`？
5. 用例函數簽名要不要順手改——例如增加無參版本，免掉 `obj? O` 與 `X!`。這件事會碰到 250 處，得你定我才動。
6. 這些樣本現在都是未跟蹤的新檔（`proj/StyleDrafts/`、`proj/Samples/Style*`、`proj/Samples/SampleDomain/`、`proj/Samples/README.typ`）。要留著、還是評估完我刪掉？

另外：`Tsinswreng.CsTreeTest/_Tswg/2026_1021_213255/Refac.typ` 我仍然沒讀（那是你自己的筆記）。如果它寫了本次重構的方向，說一聲我就照它對齊。
````
]


-T[2026_1021_221237][
	我感覺你弄的這些看起來都差不多
	都簡單不到哪去
][
````md
被擋了：這個會話現在是**只讀模式**，我寫不進新檔案（剛才兩次 `write` 都被 sandbox 拒絕）。按規矩我不會硬申請寫入，先把話講清楚；你要我動手就切回修改模式。

## 先認錯：你說得對，但原因不是「都差不多」，是我只換了一根軸

前五個樣本換的都是「**怎麼拼出樹**」的寫法（`MkTestFnRegister` / `MkRegister` / `Group`+`Case` / 檔案自報）。而真正決定「寫起來累不累」的是另一根軸：「**這棵樹由誰裝配**」。我沒碰那根軸，所以五個樣本共用同一副骨架：一個身分類、一個註冊方法、一個 mgr、一個入口，永遠都在。

還有個硬事實要講明白：**只要堅持「無反射、顯式註冊」，寫法的地板就是每條用例一行**。再往下減，只能靠反射或代碼生成。這是取捨，不是我省事。

## 三種真正更簡單的形狀（我原本要建的樣本）

### 形狀一：最省字（顯式、AOT 友善）—— 把 B 的所有零碎都刪掉

```cs
// TestAddItem.cs —— 一個被測函數一個檔案，整檔就這些
public static class TestAddItem{
	public static void AddTo(ITestNode N){
		var G = N.Group<ISvcCart>(nameof(ISvcCart.AddItem));
		G.Case(AddNewLine);
		G.Case(AddSameSkuAccumulates);
		G.Case(AddNonPositiveThrows);
	}

	static async Task AddNewLine(){
		var Svc = GetRSvc<ISvcCart>();
		await Svc.AddItem("d-add-new", 3);
		Assert.IsTrue((await Svc.GetItem("d-add-new"))?.Qty == 3, "期望數量 3");
	}
	// 其餘兩條同形
}
```

比 B 少的東西：`typeof(ISvcCart)` 不用寫（泛型給）；`nameof(AddNewLine)` 不用寫第二次（用例名由 `CallerArgumentExpression` 從源碼文本取）；`X!` 不用寫；用例函數不用吃 `obj? O`、不用 `return NIL;`；沒有 `RegisterTestsInto`；沒有測試類實例；沒有前綴。每條用例就一行 `G.Case(方法組)`。

### 形狀二：反射掃描（零註冊，JIT 友善）

```cs
// TestAddItem.cs
public static class TestAddItem{
	[TestOf(typeof(ISvcCart), nameof(ISvcCart.AddItem))]
	public static async Task AddNewLine(){ /* 同上一模一樣的用例體 */ }

	[TestOf(typeof(ISvcCart), nameof(ISvcCart.AddItem))]
	public static async Task AddSameSkuAccumulates(){ ... }
}

// Program.cs —— 全部裝配就這一行
var S = await AutoRunner.RunAssembly(typeof(TestAddItem));
```

沒有 mgr、沒有 `AddTo`、沒有 `RegisterTester`。樹由「類名 / 方法名」自動拼：`Tests/TestAddItem/AddNewLine`。代價有兩條，都不小：一是 `Assembly.GetTypes()` 加 `MethodInfo.Invoke` 對 NativeAOT 很不友善（要 trimmer 根或改委託工廠），而這個倉的測試是要 AOT 跑的；二是失去顯式控制（節點順序、`Ordered`、`IsParallelRecursive`、跨工程收編都得另外用 attribute 補）。

### 形狀三：生成樹檔（零註冊 + AOT 友善）

調用方代碼與形狀二**一字不差**，差別只在樹是構建期由腳本讀 attribute 生成 `TestTree.g.cs`，運行時不掃任何東西。你們已經有這個先例（`Ngan.Dict.Scripts` 的 `GenAppJsonCtx`）。生成物大致長這樣，人不用寫：

```cs
// 本檔由 GenTestTree 生成，勿手改
public sealed class GenTestTree : TestMgrV2{
	protected override void SetupTree(){
		var G = Root.Group<ISvcCart>(nameof(ISvcCart.AddItem));
		G.Case(TestAddItem.AddNewLine);
		G.Case(TestAddItem.AddSameSkuAccumulates);
		// …
	}
}
```

## 量化一下差距（同一批 5 條用例）

- B（上次那套）：每檔 4 行骨架，每條 1 行 `G.Case(nameof(X), X!)`，用例函數還要 `obj? O` 與 `return NIL;`，另有測試類主檔、mgr、入口。
- 形狀一：每檔 3 行骨架，每條 1 行 `G.Case(X)`，用例函數沒有 `obj? O`、沒有 `NIL`，沒有主檔。
- 形狀二 / 三：每檔 0 行骨架，每條 1 行 attribute，用例函數同形狀一，入口一行，沒有 mgr。

也就是說，形狀一已經把「每檔固定開銷」壓到 3 行；形狀二/三把固定開銷壓到 0，但換來反射或生成步驟。

## 我需要你定的兩件事

1. **反射能不能用**？能用就走形狀二（最省事，代價是 AOT）；不能用就走形狀三（代價是多一個生成腳本 + 一個 `_gen` 檔）。
2. **attribute 代替 `nameof` 能不能接受**？形狀二/三裏，用例名與被測函數名寫在 attribute 裏，方法改名時 attribute 不會報錯（只有被測函數改名會因 `nameof` 報錯）。這是靜默失效風險，得你認可。

## 要我做什麼

把這個倉的檔案策略切回**修改模式**，我就建三個新樣本 `StyleD_Terse`、`StyleE_AutoScan`、`StyleF_GenTree`，各 4 個檔、同樣 5 條用例，全部真編譯真跑（含樹路徑輸出），跟已有的 `StyleB_Static` 並排比。

已有那五個樣本還在硬盤上，現在就能看；其中 `StyleB_Static` 與 `StyleC_FileUnit` 是五個裏最省的兩個。
````
]

