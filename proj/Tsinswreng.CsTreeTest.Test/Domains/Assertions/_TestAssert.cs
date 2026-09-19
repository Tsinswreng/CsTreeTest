using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Assertions;

/// 測框架的斷言 API。
/// 本類是主檔：
/// 只負責把各分片的註冊方法串起來，
/// 用例寫在下面的分片檔裏。
public partial class TestAssert : ITester{
	public ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		RegisterIsTrue(Node);
		return Node;
	}
}