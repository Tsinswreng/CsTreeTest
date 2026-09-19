using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Index;

/// 本檔只測 TestIndex.Add：
/// 同一索引內唯一名不得重複。
public partial class TestIndexCase{
	/// 把 Add 的用例掛到節點上。
	public void RegisterAdd(ITestNode Node){
		var R = Node.MkRegister<TestIndexCase>(
			typeof(TestIndex)
			,nameof(TestIndex.Add)
			,"Index:"
		).Register;
		var T = Assert.IsTrue;

		R("索引拒絕重複的唯一名", async (o) => {
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
		});
	}
}