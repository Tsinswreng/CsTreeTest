using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 單檔：ITester 接口；純聲明，無函數可分離。
[Doc($$"""
測試類：自己持有一棵子樹，並往子樹裏填內容。

新寫法實作 `{{nameof(TestNode)}}`、{{nameof(Inited)}}、`{{nameof(Init)}}` 與 `{{nameof(AddTestCase)}}`，
通常直接繼承 `{{nameof(Tester)}}` 即可。

舊寫法只實作 `{{nameof(RegisterTestsInto)}}`，該成員已標過時，留着只為讓既有代碼繼續編譯。
舊實作不提供新成員，取用 `{{nameof(TestNode)}}` 或 `{{nameof(AddTestCase)}}` 會拋 `NotSupportedException`。
""")]
public interface ITester{
	[Doc($$"""
	本測試類的子樹根節點。

	由父方在調用 `{{nameof(Init)}}` 之前建好並賦值；
	作根使用時由入口自己賦值。

	舊寫法的測試類沒有這個概念，取用即拋 `NotSupportedException`。
	""")]
	ITestNode TestNode{
		get => throw new NotSupportedException(
			"This tester implements only the legacy RegisterTestsInto API and has no TestNode."
		);
		set => throw new NotSupportedException(
			"This tester implements only the legacy RegisterTestsInto API and has no TestNode."
		);
	}

	[Doc($$"""
	是否已經填過內容。

	為真之後不得再調用 `{{nameof(Init)}}`，同一個實例也不得再掛到別的節點下。
	舊寫法的測試類恆為假。
	""")]
	bool Inited{ get => false; set { } }

	[Doc($$"""
	往 `{{nameof(TestNode)}}` 上掛子測試類與用例。

	只掛，不跑用例；`{{nameof(TestNode)}}` 必須在調用之前已就緒。
	舊寫法的測試類此方法為空，內容在 `{{nameof(RegisterTestsInto)}}` 裏掛。
	""")]
	void Init() { }

	[Doc($$"""
	往本測試類的節點下掛一條用例。

	用例節點的唯一名即 `{{nameof(UniqName)}}`；用例本體包成 `{{nameof(TestCase)}}` 放進節點的 `{{nameof(ITestNode.Data)}}`。

	#Prm[{{nameof(UniqName)}}][用例名，全樹唯一；要能看出所測行為]
	#Prm[{{nameof(FnTest)}}][用例本體]
	#Rtn[結果，取出新建的用例節點]
	""")]
	IResAddTestCase AddTestCase(str UniqName, FnTest FnTest){
		throw new NotSupportedException(
			"This tester implements only the legacy RegisterTestsInto API and cannot add cases this way."
		);
	}

	[Obsolete($$"""
	舊寫法：把用例掛到 `{{nameof(Node)}}` 上並返回它；Node 為空時自建一個。

	新寫法請改用 `{{nameof(TestNode)}}` 與 `{{nameof(Init)}}`。
	""")]
	ITestNode RegisterTestsInto(ITestNode? Node);
}
