#import "@preview/tsinswreng-auto-heading:0.1.0": auto-heading
#let H = auto-heading;

#H[寫法樣本（評估用）][
	這批樣本是為了*從調用方代碼入手*評估 CsTreeTest 的重構方向而建的，
	不是庫的一部分，也不打算入庫。

	五個樣本做的是同一件事：測同一個被測域（購物車）的同樣五條用例，
	*只有調用方寫法不同*。所以可以直接把兩個目錄並排看，
	差異就是「換寫法要付出多少」的全部。

	#H[怎麼跑][
		每個樣本都是可執行的 Exe。在此目錄下逐個跑：

		```bash
		dotnet restore <樣本>/<樣本>.csproj -p:RestoreUseStaticGraphEvaluation=true --force
		dotnet build <樣本>/<樣本>.csproj -m:1 --no-restore
		./<樣本>/bin/Debug/net10.0/<樣本>.exe
		```

		為什麼要多那兩個參數：
		本機這個環境下 `dotnet build` 會卡在 `_GetAllRestoreProjectPathItems` 與
		`_GetProjectReferenceTargetFrameworkProperties`（嵌套 MSBuild 起不來），
		表現是「0 個錯誤卻生成失败」；`-p:RestoreUseStaticGraphEvaluation=true` 與 `-m:1`
		是繞過它的辦法，與樣本代碼無關。
	]

	#H[共用件][
		- `SampleDomain/`：被測域（`ISvcCart` 依賴 `IRepoCart`，所以才需要取依賴）、
		  共用斷言 `Chk`、全域依賴來源 `Di`（真微軟 DI，6 行核心）。
		  真實工程裏 `Di` 是*各測試工程自備*的，樣本共用一份只是免得抄五遍。
		- `../StyleDrafts/`：本次評估的*候選 API 草稿庫*。
		  它引用庫本體，只補上候選寫法需要的東西：
		  `TestMgrV2`（沒有 Init 的管理器）、`CaseGroup`（分組節點帶元數據）、
		  `ITesterStatic`（靜態測試類契約）、舊版 `0.1.1` 的仿真殼。
		  庫本體一行都沒改。
	]

	#H[五個樣本][
		#H[Style0_Legacy：升級前（NuGet 0.1.1-alpha）][
			照舊版 API 原樣寫：`DiEtTestMgr` + `InitSvc` + 建構子注入 + `MkTestFnRegister`。

			要點：
			- 測試類在容器裏註冊（`SvcColct.AddSingleton<TestCart>()`），依賴走建構子。
			- 用例掛載與「建樹」同一件事，都在 `InitSvc` 裏發生。
			- 用例註冊那 5 行是舊版形狀。

			這是後面四個樣本的對照基準。
		]

		#H[Style1_Current：庫本體現行寫法（本地 0.2.0-alpha）][
			`TestMgr` + `SetupTree` + `Init()`；測試類在*欄位初始化*時取依賴；
			用例註冊用 `MkRegister` 並帶前綴，用例體是匿名 lambda。

			要點：
			- 入口三步，順序硬性：依賴來源 → `Init()` → 跑。
			- 測試類主檔有一個 `readonly ISvcCart Svc = GetRSvc<ISvcCart>();`。
			- 每條用例寫成 `R("名字", async (O) => { ...; return NIL; });`，
			  而 `O` 幾乎都用不上。
			- 前綴（`"AddItem:"`）是為了讓用例名全樹唯一，所以路徑長。
		]

		#H[StyleA_MinDiff：最小改動][
			與 Style1 只差兩件事：管理器換成沒有 `Init()` 的版本；依賴改成*用例執行時*取。

			要點：
			- 管理器檔案：`TestMgr` → `TestMgrA`，`SetupTree` 內容一字不改。
			- 測試類主檔：依賴欄位刪掉，其餘一字不改。
			- 用例註冊那 5 行：一字不改。
			- 用例體：多一行 `var Svc = GetRSvc<ISvcCart>();`。
			- 入口：剩下兩步。

			這就是「舊調用方只改一小部分固定配置」的具體長相：
			每檔一到兩行，用例內容不動。
		]

		#H[StyleB_Static：靜態測試類 + 分組節點帶元數據][
			在 A 的基礎上換掉兩處抽象：測試類不再有實例；
			每個被測函數多一層*真節點*（分組），元數據寫在那一層。

			要點：
			- 測試類：`: ITesterStatic`，`RegisterTestsInto` 加 `static`，
			  管理器不 `new` 任何東西。
			- 每個被測函數的檔案：開頭一行建分組並寫元數據，
			  之後每條用例只寫 `G.Case(nameof(X), X!);`。
			- 用例是*常態具名函數*（`static async Task<nil> X(obj? O)`），
			  不用匿名 lambda；`obj? O` 這條簽名著實無用，但它是框架的
			  用例函數約定（`FnTest`）。
			- 不需要前綴：路徑由分組名承擔。
			  樹長這樣：`Tests/Cart/TestCart/AddItem/AddNewLine`。
			- 分組是節點，所以可以*只把一個被測函數*的用例串行化
			  （`G.Node.Ordered = true;`），不必把整個測試類都串起來。
			- 篩選照舊可用（樣本裏按被測函數名挑出 2 條）。

			已知的疙瘩：
			- 具名用例要寫 `X!`（`Task<nil>` 與 `FnTest` 的可空性不同），
			  這是庫本體的既有形狀，不是本寫法引入的。
			- 分組節點名默認取被測函數名，而節點唯一名是*全樹*唯一；
			  兩個被測類型有同名函數時要自己給 `NodeName`。
		]

		#H[StyleC_FileUnit：以檔案為註冊單位][
			沒有測試類、沒有主檔。管理器直接列每個檔案的 `Register`。
			樹上沒有「測試類」那一層：`Tests/Cart/AddItem/AddNewLine`。

			要點：
			- 檔案：`public static class TestAddItem { public static void Register(ITestNode Node){...} }`，
			  一個檔案就是一個完整單位。
			- 管理器：`var Cart = AddBranch("Cart"); TestAddItem.Register(Cart); ...`
			  一眼看全本工程有哪些測試檔。
			- 元數據與 B 一樣寫在分組上。篩選仍可按被測類型、被測函數名做。

			代價：
			- 沒有「測試類」這個可查詢維度（`Filter().TesterType(...)` 用不上），
			  失敗輸出裏的測試類名也顯示不出來。
			- 管理器檔案會隨被測函數數量線性變長。
		]
	]

	#H[把兩個樣本直接對照][
		#H[同一條用例，四種寫法][
			Style0（舊版）：
			```cs
			var register = Node.MkTestFnRegister(
				typeof(TestCart), [typeof(ISvcCart)], [nameof(ISvcCart.AddItem)], "AddItem:");
			var R = register.Register;
			R("加入新條目", async (O) => {
				await Svc.AddItem("s0-add-new", 3);
				var Line = await Svc.GetItem("s0-add-new");
				T(Line?.Qty == 3, "期望數量 3");
				return null;
			});
			```

			Style1（現行）：同形，只是 `MkTestFnRegister` 改名 `MkRegister`、
			依賴從建構子改成 `GetRSvc` 欄位、`null` 改成 `NIL`。

			StyleA（最小改動）：同 Style1，用例體多一行 `var Svc = GetRSvc<ISvcCart>();`。

			StyleB（推薦方向）：
			```cs
			static void RegisterAddItem(ITestNode Node){
				var G = Node.Group<TestCart>(typeof(ISvcCart), nameof(ISvcCart.AddItem));
				G.Case(nameof(AddNewLine), AddNewLine!);
			}

			static async Task<nil> AddNewLine(obj? O){
				var Svc = GetRSvc<ISvcCart>();
				await Svc.AddItem("sb-add-new", 3);
				Assert.IsTrue((await Svc.GetItem("sb-add-new"))?.Qty == 3, "期望數量 3");
				return NIL;
			}
			```

			StyleC（檔案自報）：同 B，但把 `TestCart` 這層去掉，
			`Register` 改名 `public static void Register(ITestNode Node)` 由管理器直接調。
		]

		#H[每個檔案要寫的固定行數（同一批 5 條用例）][
			- Style0：主檔 無依賴欄位 + ctor（約 8 行固定）、每檔 6 行註冊骨架 + 每條用例 1 行 `R(...)`。
			- Style1：入口 3 步、主檔 依賴欄位 + 8 行、每檔 6 行骨架 + 每條 1 行。
			- StyleA：入口 2 步、主檔 6 行、其餘同 Style1。
			- StyleB：入口 2 步、主檔 5 行、每檔 3 行骨架（分組 + 每條 1 行 `G.Case`）+ 每條用例一個具名函數。
			- StyleC：入口 2 步、無主檔、每檔 3 行骨架、管理器每檔一行。
		]
	]

	#H[從舊版升級要改多少][
		按樣本逐檔數出來的結論：

		- 用例註冊那幾行*可以完全不動*——只要新版保留 `MkTestFnRegister` 這個名字作別名
		  （舊版 250 處調用點大多長這個樣子）。
		- 每個測試類主檔：把依賴欄位改成用例內 `GetRSvc`（或用 `static` 欄位，但那樣時序約束還在）。
		- 每個測試類主檔：`: ITester` 與 `RegisterTestsInto` 依選定寫法決定要不要加 `static`。
		- 每個 `*TestMgr.cs`：基底類換一次；`InitSvc` 那一行刪掉。
		- 每個入口 `Program.cs`：`Mgr.Init()` 這一行刪掉。

		沒有一項需要改用例名、用例體、斷言或分檔方式。
	]

	#H[待拍板][
		- 選 A、B 還是 C（或 A 先上、B 再上）。
		- 若選 B：`ITesterStatic` 這個名字能不能接受（庫本體現有 `ITester` 是實例契約）。
		- 舊版名字（`MkTestFnRegister`、`DiEtTestMgr`、`InitSvc`）要不要留別名。
		- `CaseGroup` 這種「分組即節點」的形狀要不要進庫本體，
		  以及分組節點名的唯一性怎麼約定。
	]
]
