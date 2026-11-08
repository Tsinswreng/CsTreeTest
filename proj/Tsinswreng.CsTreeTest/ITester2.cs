using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 單檔：ITester2 接口；純聲明，無函數可分離。
[Doc($$"""
測試類：自己持有一棵子樹，並往子樹裏填內容。

與 `{{nameof(ITester)}}` 的分工：
`{{nameof(ITester)}}` 由父方建好節點再交給測試類，測試類掛完把節點返回；
`{{nameof(ITester2)}}` 的測試類自己是子樹的所有者，父方掛它時先把節點交給它，再由 `{{nameof(Init)}}` 往裏填。

節點上的測試類類型由父方按泛型參數寫入，所以按測試類篩選不必靠運行時反射去猜。
""")]
public interface ITester2{
	[Doc($$"""
	本測試類的子樹根節點。

	由父方在調用 `{{nameof(Init)}}` 之前建好並賦值；
	作根使用時由入口自己賦值。
	""")]
	ITestNode TestNode{get;set;}

	[Doc($$"""
	是否已經填過內容。

	為真之後不得再調用 `{{nameof(Init)}}`，同一個實例也不得再掛到別的節點下。
	""")]
	bool Inited{get;set;}

	[Doc($$"""
	往 `{{nameof(TestNode)}}` 上掛子測試類與用例。

	只掛，不跑用例；`{{nameof(TestNode)}}` 必須在調用之前已就緒。
	""")]
	void Init();
}
