using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Index;

// 分片聲明文件：本片測試 TestIndex.Add；實現見 TestAdd.Impl.cs。
[Doc($$"""
#Sum[本檔只測 `TestIndex.Add`：
同一索引內唯一名不得重複。]
#Descr[
*本檔只測這一個被測函數*，
新增別的被測函數的測試時請另開文件。
]
""")]
public partial class TestIndexCase{
	[Doc($$"""
#Sum[把 `Add` 的用例掛到節點上。]
#Params([[要掛用例的節點]])
""")]
	public partial void RegisterAdd(ITestNode Node);

	[Doc($$"""
#Sum[往同一索引裏加第二個同名節點時 `Add` 拋異常，即索引拒絕重複的唯一名。]
""")]
	public partial Task<nil> Add_DuplicateUniqName_Throws(obj? O);
}