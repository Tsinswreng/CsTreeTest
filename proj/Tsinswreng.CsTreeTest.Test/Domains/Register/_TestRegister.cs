using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Register;

/// 測註冊器（MkRegister / TestFnRegister）。
///
/// 本類同時演示「測試類從全局依賴來源取被測對象」：
/// 欄位初始化時直接 GetRSvc，
/// 不需要建構子參數。
/// 本類是主檔：
/// 只負責把各被測函數的用例檔串起來，
/// 自己不寫用例。
public partial class TestRegister : ITester{
	/// 被測對象。
	private readonly IFakeSvc _Svc = GetRSvc<IFakeSvc>();

	public ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterMkRegister(Node);
		return Node;
	}
}