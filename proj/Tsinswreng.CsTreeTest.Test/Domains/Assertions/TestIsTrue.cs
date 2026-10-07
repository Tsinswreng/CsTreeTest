using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Assertions;

// 分片聲明文件：本片測試 Assert.IsTrue；實現見 TestIsTrue.Impl.cs。
[Doc($$"""
#Sum[測 `{{nameof(Assert.IsTrue)}}` 的分片。]
#Descr[
*本檔只測 `{{nameof(Assert.IsTrue)}}` 這一個被測函數。*
新增別的函數的測試時請另開文件，
不要把不同函數的用例堆在同一檔。
]
""")]
public partial class TestAssert{
	[Doc($$"""
#Sum[把 `{{nameof(Assert.IsTrue)}}` 的用例掛到節點上。]
#Params([[要掛用例的節點]])
""")]
	public partial void RegisterIsTrue(ITestNode Node);

	[Doc($$"""
#Sum[條件成立時 `{{nameof(Assert.IsTrue)}}` 什麼都不做，正常返回 `NIL`。]
""")]
	public partial Task<nil> IsTrue_ConditionHolds_NoThrow(obj? O);

	[Doc($$"""
#Sum[條件不成立時 `{{nameof(Assert.IsTrue)}}` 拋異常，異常被執行器記爲該用例失敗。]
""")]
	public partial Task<nil> IsTrue_ConditionFails_Throws(obj? O);
}