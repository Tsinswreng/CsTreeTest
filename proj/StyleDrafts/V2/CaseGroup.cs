using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Drafts;

/// <summary>
/// 一個「被測函數分組」：掛在測試類節點底下，自己是一層真實的樹節點。
/// </summary>
/// <remarks>
/// 它解決的是「元數據每檔寫一遍」這件事：
/// 屬於哪個測試類、測哪個被測類型、測哪個被測函數名，
/// 在建分組時寫一次，之後同檔的每條用例只寫名字與函數體。
///
/// 分組是*真節點*，所以它也能單獨設 <see cref="ITestNode.Ordered"/> 與
/// <see cref="ITestNode.NonParallelizable"/>：
/// 一個被測函數的用例要串行（例如共用同一份測試數據）就標在這一層，
/// 不必把整個測試類都串起來。
///
/// 節點名默認取被測函數名，因此*必須在同一棵樹內唯一*；
/// 同一個函數名在兩個被測類型下都出現時，用 <c>NodeName</c> 參數區分。
/// </remarks>
public sealed class CaseGroup{
	public CaseGroup(ITestNode Node, Type? TesterType, Type TesteeType, str TesteeFnName){
		ArgumentNullException.ThrowIfNull(Node);
		ArgumentNullException.ThrowIfNull(TesteeType);
		ArgumentNullException.ThrowIfNull(TesteeFnName);
		this.Node = Node;
		this.TesterType = TesterType;
		this.TesteeType = TesteeType;
		this.TesteeFnName = TesteeFnName;
	}

	/// <summary>本分組的樹節點。要改順序／並行就改它。</summary>
	public ITestNode Node{ get; }

	/// <summary>所屬測試類類型；無測試類概念時（檔案自報式）為空。</summary>
	public Type? TesterType{ get; }

	/// <summary>本分組測的被測類型。</summary>
	public Type TesteeType{ get; }

	/// <summary>本分組測的被測函數名。</summary>
	public str TesteeFnName{ get; }

	/// <summary>掛一條用例，用例函數帶框架參數（<c>(obj? O)</c>）。</summary>
	/// <param name="UniqName">用例名，同樹內唯一。</param>
	/// <param name="Fn">用例函數本體。</param>
	public void Case(str UniqName, FnTest Fn) => Add(UniqName, Fn);

	/// <summary>掛一條用例，用例函數不帶參數、非同步。</summary>
	/// <param name="UniqName">用例名，同樹內唯一。</param>
	/// <param name="Fn">用例函數本體。</param>
	public void Case(str UniqName, Func<Task> Fn) => Add(UniqName, async _ => {
		await Fn();
		return NIL;
	});

	/// <summary>掛一條用例，用例函數不帶參數、同步。</summary>
	/// <param name="UniqName">用例名，同樹內唯一。</param>
	/// <param name="Fn">用例函數本體。</param>
	public void Case(str UniqName, Action Fn) => Add(UniqName, _ => {
		Fn();
		return Task.FromResult<obj?>(NIL);
	});

	private void Add(str UniqName, FnTest Fn){
		ArgumentNullException.ThrowIfNull(UniqName);
		ArgumentNullException.ThrowIfNull(Fn);
		var Case = new TestCase{
			UniqName = UniqName,
			TesterType = TesterType,
			TesteeTypes = [TesteeType],
			TesteeFnNames = [TesteeFnName],
			FnTest = Fn,
		};
		Node.Children.Add(new TestNode{
			UniqName = UniqName,
			Data = Case,
			Parents = [Node],
		});
	}
}

/// <summary>掛用例的動作；「檔案自報式」用它把一個檔案的用例整批交給父節點。</summary>
/// <param name="Node">用例要掛上去的那個節點。</param>
public delegate void FnCasesInto(ITestNode Node);

/// <summary>建分組的入口。</summary>
public static class ExtnCaseGroup{
	/// <summary>建一個分組，所屬測試類由泛型參數給。</summary>
	/// <typeparam name="TTester">測試類類型。</typeparam>
	/// <param name="z">分組的父節點，即 <c>this</c>。</param>
	/// <param name="TesteeType">被測類型。</param>
	/// <param name="TesteeFnName">被測函數名，必須用 <c>nameof</c>。</param>
	/// <param name="NodeName">分組節點名；為空時取被測函數名。</param>
	public static CaseGroup Group<TTester>(
		this ITestNode z
		,Type TesteeType
		,str TesteeFnName
		,str? NodeName = null
	){
		return Mk(z, typeof(TTester), TesteeType, TesteeFnName, NodeName);
	}

	/// <summary>建一個分組，不指明所屬測試類（檔案自報式用）。</summary>
	/// <param name="z">分組的父節點，即 <c>this</c>。</param>
	/// <param name="TesteeType">被測類型。</param>
	/// <param name="TesteeFnName">被測函數名，必須用 <c>nameof</c>。</param>
	/// <param name="NodeName">分組節點名；為空時取被測函數名。</param>
	public static CaseGroup Group(
		this ITestNode z
		,Type TesteeType
		,str TesteeFnName
		,str? NodeName = null
	){
		return Mk(z, null, TesteeType, TesteeFnName, NodeName);
	}

	private static CaseGroup Mk(
		ITestNode z
		,Type? TesterType
		,Type TesteeType
		,str TesteeFnName
		,str? NodeName
	){
		ArgumentNullException.ThrowIfNull(z);
		ArgumentNullException.ThrowIfNull(TesteeType);
		ArgumentNullException.ThrowIfNull(TesteeFnName);
		var Node = z.NewChild(NodeName ?? TesteeFnName);
		return new CaseGroup(Node, TesterType, TesteeType, TesteeFnName);
	}
}
