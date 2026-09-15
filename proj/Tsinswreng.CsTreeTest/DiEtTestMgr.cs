using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

[Doc(@$"DependencyInjection and Test Manager.
Own a {nameof(TestNode)}
Do not own {nameof(IServiceCollection)} and {nameof(IServiceProvider)}.
each test csproj should have one {nameof(IDiEtTestMgr)}.
{nameof(IDiEtTestMgr)} itself can be registered by other {nameof(IDiEtTestMgr)}
")]
public interface IDiEtTestMgr:ITester{
	[Doc(@$"the value list should only contain 0 or 1 element,
	designed as IList is for future extension
	")]
	public IDictionary<str, IList<ITestNode>> UniqName_TestNode{get;set;}
	
	[Doc(@$"the value list should only contain 0 or 1 element,
	designed as IList is for future extension
	")]
	public IDictionary<Type, IList<ITestNode>> TesterType_TestNode{get;set;}
	
	[Doc(@$"One To Many")]
	public IDictionary<Type, IList<ITestNode>> TesteeType_TestNode{get;set;}
	
	[Doc(@$"One To Many")]
	public IDictionary<str, IList<ITestNode>> TesteeFnName_TestNode{get;set;}
	
	[Doc(@$"Root TestNode of the (Sub)Tree")]
	public ITestNode TestNode{get;set;}
	
	[Doc(@$"Functions to register types into {nameof(IServiceCollection)}")]
	public IList<Func<IServiceCollection, nil>> DiFns{get;set;}
	
	[Doc(@$"Functions to retrieve {nameof(ITester)}
	from {nameof(IServiceProvider)} and register it into {nameof(TestNode)}
	")]
	public IList<Func<IServiceProvider, ITestNode, nil>> RegisterTestFns{get;set;}
}

// 聲明文件：屬性 + 構造函數聲明（partial method）；實現見 DiEtTestMgr.Impl.cs。
public abstract partial class DiEtTestMgr:IDiEtTestMgr{
	public partial DiEtTestMgr();
	public IDictionary<str, IList<ITestNode>> UniqName_TestNode{get;set;} = new Dictionary<str, IList<ITestNode>>();
	public IDictionary<Type, IList<ITestNode>> TesterType_TestNode{get;set;} = new Dictionary<Type, IList<ITestNode>>();
	public IDictionary<Type, IList<ITestNode>> TesteeType_TestNode{get;set;} = new Dictionary<Type, IList<ITestNode>>();
	public IDictionary<str, IList<ITestNode>> TesteeFnName_TestNode{get;set;} = new Dictionary<str, IList<ITestNode>>();
	public ITestNode TestNode{get;set;} = new TestNode();
	public IList<Func<IServiceCollection, nil>> DiFns{get;set;} = [];
	public IList<Func<IServiceProvider, ITestNode, nil>> RegisterTestFns{get;set;} = [];
	public abstract ITestNode RegisterTestsInto(ITestNode? Test);
}

// IDiEtTestMgr 的擴展：註冊/索引/初始化。
// 聲明文件：傳統 this 擴展（調用方寫法與 extension 塊一致）+ partial method 拆分；實現見 DiEtTestMgr.Impl.cs。
public static partial class ExtnDiEtTestMgr{
	[Doc($$"""
	Register an impl class of {{nameof(ITester)}}
	both into {{nameof(ServiceCollection)}} and {{nameof(TestNode)}}
	""")]
	public static partial void RegisterTester<
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
		T
	>(this IDiEtTestMgr z, str? NewNodeUniqName = null)
		where T:class, ITester;

	[Doc(@$"Register another {nameof(IDiEtTestMgr)} as self's subnode")]
	public static partial void RegisterSubMgr(this IDiEtTestMgr z, IDiEtTestMgr SubMgr);
	
	[Doc(@$"Init {nameof(IServiceCollection)} and {nameof(IServiceProvider)}
	you should provide the two args by yourself, then better call this at entrance
	")]
	public static partial IServiceProvider InitSvc(
		this IDiEtTestMgr z
		,IServiceCollection SvcColct
		,IServiceProvider SvcProvdr
	);

	[Doc(@$"Init {nameof(IServiceCollection)} then build and return {nameof(IServiceProvider)}")]
	public static partial IServiceProvider InitSvc(
		this IDiEtTestMgr z
		,IServiceCollection SvcColct
		,Func<IServiceCollection, IServiceProvider> BuildSvcProvider
	);
	
	public static partial ITestNode GetNodeByName(this IDiEtTestMgr z, str UniqName);
	
	public static partial ITestNode GetNodeByTesterType(this IDiEtTestMgr z, Type TesterType);
	
	[Obsolete(@$"Use {nameof(GetNodesByTesteeType)}")]
	public static partial ITestNode GetNodeByTesteeType(this IDiEtTestMgr z, Type TesteeType);
	
	[Obsolete(@$"Use {nameof(GetNodesByTesteeFnName)}")]
	public static partial ITestNode GetNodeByTesteeFnName(this IDiEtTestMgr z, str TesteeFnName);
	
	public static partial IList<ITestNode> GetNodesByTesteeType(this IDiEtTestMgr z, Type TesteeType);
	
	public static partial IList<ITestNode> GetNodesByTesteeFnName(this IDiEtTestMgr z, str TesteeFnName);

	/// 把子樹裡帶 metadata 的用例節點按 TesteeTypes/TesteeFnNames 入索引。
	private static partial void IndexTesteeForSubtree(IDiEtTestMgr z, ITestNode Root);

	/// 迭代子樹所有節點（先序）。
	private static partial IEnumerable<ITestNode> WalkNodes(ITestNode Root);

	/// 往字典的列表值追加節點（去重）。
	private static partial void AddNodeToDict<TKey>(
		IDictionary<TKey, IList<ITestNode>> Dict
		,TKey Key
		,ITestNode Node
	) where TKey : notnull;
}