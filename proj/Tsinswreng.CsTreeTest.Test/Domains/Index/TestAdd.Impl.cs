using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Index;

// 實現文件：只放函數實現。
public partial class TestIndexCase{
	public partial void RegisterAdd(ITestNode Node){
		var R = Node.MkRegister<TestIndexCase>(
			typeof(TestIndex)
			,nameof(TestIndex.Add)
			,"Index:"
		).Register;

		R(nameof(Add_DuplicateUniqName_Throws), Add_DuplicateUniqName_Throws!);
	}

	public partial async Task<nil> Add_DuplicateUniqName_Throws(obj? O){
		var T = Assert.IsTrue;
		var Idx = new TestIndex();
		Idx.Add(new TestNode{UniqName = "Dup"});
		var Thrown = false;
		try{
			Idx.Add(new TestNode{UniqName = "Dup"});
		}catch{
			Thrown = true;
		}
		T(Thrown);
		return NIL;
	}
}