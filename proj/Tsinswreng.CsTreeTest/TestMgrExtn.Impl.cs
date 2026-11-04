using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public static partial class ExtnTestMgr{
	public static partial IEnumerable<ITestNode> Walk(this ITestNode z){
		yield return z;
		foreach(var Child in z.Children){
			foreach(var Descendant in Child.Walk()){
				yield return Descendant;
			}
		}
	}

	public static partial IEnumerable<ITestNode> WalkCases(this ITestNode z){
		foreach(var Node in z.Walk()){
			if(Node.Data is not null){
				yield return Node;
			}
		}
	}

	public static partial ITestNode GetNodeByTesterType(this ITestMgr z, Type TesterType){
		return z.Index.FindByTesterType(TesterType)
			?? throw new KeyNotFoundException($"找不到該測試類的節點：{TesterType.FullName}");
	}

	public static partial ITestNode GetNodeByName(this ITestMgr z, str UniqName){
		return z.Index.FindByName(UniqName)
			?? throw new KeyNotFoundException($"找不到該名字的節點：{UniqName}");
	}

	public static partial IList<ITestNode> GetNodesByTesteeType(this ITestMgr z, Type TesteeType){
		var R = z.Index.FindByTesteeType(TesteeType);
		if(R.Count < 1){
			throw new KeyNotFoundException($"找不到測該類型的用例：{TesteeType.FullName}");
		}
		return R;
	}

	public static partial IList<ITestNode> GetNodesByTesteeFnName(this ITestMgr z, str TesteeFnName){
		var R = z.Index.FindByTesteeFnName(TesteeFnName);
		if(R.Count < 1){
			throw new KeyNotFoundException($"找不到測該函數的用例：{TesteeFnName}");
		}
		return R;
	}

	public static partial TestFilter Filter(this ITestMgr z){
		return new TestFilter(z);
	}

	public static partial TestFilter Filter(this ITestMgr z, ITestNode Scope){
		return new TestFilter(z, Scope);
	}
}
