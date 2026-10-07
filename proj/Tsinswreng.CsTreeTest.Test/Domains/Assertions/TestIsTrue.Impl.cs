using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Assertions;

// 實現文件：只放函數實現。
public partial class TestAssert{
	public partial void RegisterIsTrue(ITestNode Node){
		var R = Node.MkRegister<TestAssert>(
			typeof(Assert)
			,nameof(Assert.IsTrue)
			,"Assert:"
		).Register;

		R(nameof(IsTrue_ConditionHolds_NoThrow), IsTrue_ConditionHolds_NoThrow!);
		R(nameof(IsTrue_ConditionFails_Throws), IsTrue_ConditionFails_Throws!);
	}

	public partial async Task<nil> IsTrue_ConditionHolds_NoThrow(obj? O){
		var T = Assert.IsTrue;
		T(true);
		T(1 == 1);
		return NIL;
	}

	public partial async Task<nil> IsTrue_ConditionFails_Throws(obj? O){
		var T = Assert.IsTrue;
		var Thrown = false;
		try{
			Assert.IsTrue(false, "故意的失敗");
		}catch{
			Thrown = true;
		}
		T(Thrown);
		return NIL;
	}
}