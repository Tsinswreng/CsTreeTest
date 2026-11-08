using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public static partial class ITesterExtn{
	public static partial IResAddTestCase AddTestCase(
		this ITester z
		,str UniqName
		,FnTest FnTest
	){
		// 能力在節點上，這裏只負責把「本測試類的節點」指出來。
		return z.TestNode.AddTestCase(UniqName, FnTest);
	}

	public static partial void AddTester<TTester>(this ITester z) where TTester: ITester, new(){
		z.AddTester(new TTester(), typeof(TTester));
	}

	public static partial void AddTester(
		this ITester z
		,ITester Tester
		,Type TesterType
	){
		// 1: 已經填過內容的實例不能再掛：再掛一次會讓同一批用例出現在兩棵樹裏。
		if(Tester.Inited){
			throw new InvalidOperationException(
				"Tester has already been initialized and cannot be attached twice."
			);
		}
		// 2: 節點由父方建，TesterType 寫進節點，節點名默認取類型短名。
		var node = z.TestNode.NewTesterChild(TesterType);
		// 3: 先交節點再初始化，子方才能往自己那個節點上掛東西。
		Tester.TestNode = node;
		Tester.Init();
		Tester.Inited = true;
	}
}
