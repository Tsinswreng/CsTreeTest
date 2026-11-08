using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public static partial class ITestNodeExtn{
	public static partial ITestNode NewChild(this ITestNode z, str? UniqName){
		var R = new TestNode{
			UniqName = UniqName ?? new Guid().ToString(),//U128Id.NewUlid().ToLow64Base(),
		};
		z.Children.Add(R);
		R.Parents.Add(z);
		return R;
	}

	public static partial ITesterNode NewTesterChild<T>(this ITestNode z, str? UniqName) where T:ITester{
		return z.NewTesterChild(typeof(T), UniqName);
	}

	public static partial ITesterNode NewTesterChild(this ITestNode z, Type TesterType, str? UniqName){
		ArgumentNullException.ThrowIfNull(TesterType);
		var R = new TesterNode{
			UniqName = UniqName ?? TesterType.Name,
			TesterType = TesterType,
		};
		z.Children.Add(R);
		R.Parents.Add(z);
		return R;
	}

	public static partial IResAddTestCase AddTestCase(this ITestNode z, str UniqName, FnTest FnTest){
		// 1: 造用例。用例名在節點上，不必另存一份。
		var testCase = new TestCase{
			FnTest = FnTest,
		};
		// 2: 掛成子節點；父子兩邊的引用都要補。
		var newNode = new TestNode{
			UniqName = UniqName,
			Data = testCase,
			Parents = [z],
		};
		z.Children.Add(newNode);
		// 3: 把新節點交回調用方，方便它接着設旗標或掛子節點。
		return new ResAddTestCase{
			NewTestNode = newNode,
		};
	}

	public static partial IEnumerable<ITestNode> Walk(this ITestNode z){
		yield return z;
		foreach(var child in z.Children){
			foreach(var descendant in child.Walk()){
				yield return descendant;
			}
		}
	}

	public static partial IEnumerable<ITestNode> WalkCases(this ITestNode z){
		foreach(var node in z.Walk()){
			if(node.Data is not null){
				yield return node;
			}
		}
	}

	public static partial IList<str> PathFromRoot(this ITestNode z){
		var R = new List<str>();
		ITestNode? Cur = z;
		// 樹正常時不會成環；
		// 加步數上限是為了防止使用者手工接出環之後死循環。
		var Guard = 0;
		while(Cur is not null && Guard++ < 1000){
			R.Add(Cur.UniqName);
			Cur = Cur.Parents.Count > 0 ? Cur.Parents[0] : null;
		}
		R.Reverse();
		return R;
	}

	public static partial str PathString(this ITestNode z){
		return string.Join("/", z.PathFromRoot());
	}
}
