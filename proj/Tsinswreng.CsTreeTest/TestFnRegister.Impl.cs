using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public partial class TestFnRegister{
	public partial TestFnRegister(
		ITestNode Node
		,Type TesterType
		,IList<Type> TesteeTypes
		,IList<str> TesteeFnNames
		,str UniqNamePrefix
	){
		this.Node = Node;
		this.TesterType = TesterType;
		this.TesteeTypes = TesteeTypes;
		this.TesteeFnNames = TesteeFnNames;
		this.UniqNamePrefix = UniqNamePrefix;
	}

	public partial void Register(str UniqName, FnTest Fn){
		var Case = new TestCase{
			UniqName = UniqNamePrefix + UniqName,
			TesterType = TesterType,
			TesteeTypes = TesteeTypes,
			TesteeFnNames = TesteeFnNames,
			FnTest = Fn,
		};
		this.Node.Children.Add(new TestNode{
			UniqName = Case.UniqName,
			Data = Case,
			Parents = [this.Node],
		});
	}
}
