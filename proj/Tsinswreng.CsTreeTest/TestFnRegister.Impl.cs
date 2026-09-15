namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public partial class TestFnRegister : ITestFnRegister{
	public partial TestFnRegister(ITestNode node, Type testerType, Type testeeType, str uniqNamePrefix){
		Node = node;
		TesterType = testerType;
		TesteeTypes = [testeeType];
		TesteeFnNames = [];
		UniqNamePrefix = uniqNamePrefix;
	}
	
	public partial TestFnRegister(
		ITestNode node
		,Type testerType
		,IList<Type> testeeTypes
		,IList<str> testeeFnNames
		,str uniqNamePrefix
	){
		Node = node;
		TesterType = testerType;
		TesteeTypes = testeeTypes;
		TesteeFnNames = testeeFnNames;
		UniqNamePrefix = uniqNamePrefix;
	}

	public partial void Register(str UniqName, FnTest Fn){
		var Case = new TestCase{
			UniqName = UniqNamePrefix + UniqName,
			TesterType = TesterType,
			TesteeTypes = TesteeTypes,
			TesteeFnNames = TesteeFnNames,
			FnTest = Fn,
		};
		Node.Children.Add(new TestNode{
			Data = Case,
		});
	}
}