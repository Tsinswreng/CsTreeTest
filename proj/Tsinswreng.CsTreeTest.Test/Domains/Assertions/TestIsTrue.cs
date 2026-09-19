using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Assertions;

/// 測 Assert.IsTrue 的分片。
public partial class TestAssert{
	/// 把 IsTrue 的用例掛到節點上。
	public void RegisterIsTrue(ITestNode Node){
		var R = Node.MkRegister<TestAssert>(
			typeof(Assert)
			,nameof(Assert.IsTrue)
			,"Assert:"
		).Register;
		var T = Assert.IsTrue;

		R("成立時不拋", async (o) => {
			T(true);
			T(1 == 1);
			return NIL;
		});

		R("不成立時拋出", async (o) => {
			var Thrown = false;
			try{
				Assert.IsTrue(false, "故意的失敗");
			}catch{
				Thrown = true;
			}
			T(Thrown);
			return NIL;
		});
	}
}