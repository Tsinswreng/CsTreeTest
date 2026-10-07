using Tsinswreng.CsCore;
using Tsinswreng.CsU128Id;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public static partial class ExtnITestNode{
	public static partial ITestNode NewChild(this ITestNode z, str? UniqName){
		var R = new TestNode{
			UniqName = UniqName ?? U128Id.NewUlid().ToLow64Base(),
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