using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 聲明文件：ITesterExtn 擴展方法的宣告；實現見 ITesterExtn.Impl.cs。
[Doc($$"""
`{{nameof(ITester)}}` 上「往自己的節點掛東西」的擴展方法。

能力本身都在 `{{nameof(ITestNodeExtn)}}` 上，本類只是把「往自己的節點上掛」說短，並轉發過去。
舊寫法的測試類沒有 `{{nameof(ITester.TestNode)}}`，傳進來會在取節點時拋 `NotSupportedException`。
""")]
public static partial class ITesterExtn{
	[Doc($$"""
	往本測試類的節點下掛一條用例。

	轉發給 `{{nameof(ITestNodeExtn.AddTestCase)}}`。

	#Prm[{{nameof(UniqName)}}][用例名，全樹唯一；要能看出所測行為]
	#Prm[{{nameof(FnTest)}}][用例本體]
	#Rtn[結果，取出新建的用例節點]
	""")]
	public static partial IResAddTestCase AddTestCase(
		this ITester z
		,str UniqName
		,FnTest FnTest
	);

	[Doc($$"""
	把一個子測試類掛到本測試類的節點下，類型由泛型參數給。

	本方法負責建子節點，節點名默認取 `{{nameof(TTester)}}` 的短名。
	節點建好後交給子方，再由子方的 `{{nameof(ITester.Init)}}` 填內容。

	#TPrm[{{nameof(TTester)}}][子測試類類型，須有無參構造子]
	""")]
	public static partial void AddTester<TTester>(this ITester z) where TTester: ITester, new();

	//TswgNote 記錄 TesterType 信息
	[Doc($$"""
	同上，實例與類型都由調用方給，用於運行時才知道測試類型的場合。

	同一個實例只能掛一次，重複掛拋 `InvalidOperationException`。

	#Prm[{{nameof(Tester)}}][子測試類實例]
	#Prm[{{nameof(TesterType)}}][子測試類類型，用作子節點的默認名]
	""")]
	public static partial void AddTester(
		this ITester z
		,ITester Tester
		,Type TesterType
	);
}
