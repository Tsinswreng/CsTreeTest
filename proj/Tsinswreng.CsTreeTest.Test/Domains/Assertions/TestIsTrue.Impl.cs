using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Assertions;

public partial class TestAssert{
	public partial void RegisterIsTrue(ITestNode Node){
		var R = Node.MkTestFnRegister<TestAssert>().Register;
		var T = Assert.IsTrue;

		R("IsTrue(true) 不拋", async (o)=>{
			Assert.IsTrue(true);
			return null;
		});

		R("IsTrue(false) 拋異常且帶自定義消息", async (o)=>{
			try{
				Assert.IsTrue(false, "custom message");
				T(false);
			}catch(Exception Ex){
				T(Ex.Message == "custom message");
			}
			return null;
		});

		R("IsTrue(false) 默認消息為空", async (o)=>{
			try{
				Assert.IsTrue(false);
				T(false);
			}catch(Exception Ex){
				T(Ex.Message == "");
			}
			return null;
		});
	}
}