using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Register;

// 分片聲明文件：本片測試 ExtnITestFnRegister.MkRegister 的元數據流轉；實現見 TestMkRegister.Impl.cs。
[Doc($$"""
#Sum[測 `{{nameof(ITestFnRegisterExtn.MkRegister)}}` 的元數據如何流進用例。]
#Descr[
*本檔主要測 `ExtnITestFnRegister.MkRegister`。*
註冊時給的測試類類型、被測類型、被測函數名、用例名前綴
都會被逐條快照到用例上，
本檔就是驗這件事。
]
""")]
public partial class TestRegister{
	[Doc($$"""
#Sum[把元數據用例掛到節點上。]
#Descr[
本領域測的就是「元數據會帶進用例」，
因此本檔自己就用這個機制註冊用例。
]
#Params([[要掛用例的節點]])
""")]
	public partial void RegisterMkRegister(ITestNode Node);

	[Doc($$"""
#Sum[測試類欄位從全局依賴來源取到的服務可用，原樣返回輸入。]
""")]
	public partial Task<nil> MkRegister_SvcFromGlobalDi_Usable(obj? O);

	[Doc($$"""
#Sum[按註冊器設定註冊的用例，其測試類類型、被測類型、被測函數名都與設定一致。]
""")]
	public partial Task<nil> MkRegister_WithMetadata_MetadataCopiedIntoCase(obj? O);

	[Doc($$"""
#Sum[不傳元數據時，用例的被測類型與被測函數名列表皆為空。]
""")]
	public partial Task<nil> MkRegister_NoMetadata_TesteeListsEmpty(obj? O);
}