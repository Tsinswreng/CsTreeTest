using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public abstract partial class DiEtTestMgr{
	public partial DiEtTestMgr(){
		this.RegisterTestsInto(this.TestNode);
	}
}

public static partial class ExtnDiEtTestMgr{
	public static partial void RegisterTester<T>(
		this IDiEtTestMgr z, str? NewNodeUniqName)
		where T:class, ITester
	{
		z.DiFns.Add((SvcC)=>{
			SvcC.Add(ServiceDescriptor.Singleton(typeof(T), typeof(T)));
			return NIL;
		});
		z.RegisterTestFns.Add((SvcP, TestNode)=>{
			if(z.TesterType_TestNode.ContainsKey(typeof(T))){
				throw new InvalidOperationException(
					$"Duplicated tester type: {typeof(T)}"
				);
			}
			if(NewNodeUniqName != null && z.UniqName_TestNode.ContainsKey(NewNodeUniqName)){
				throw new InvalidOperationException($"Duplicated test node uniq name: {NewNodeUniqName}");
			}
			ITester t = SvcP.GetRequiredService<T>();
			ITestNode node = TestNode.NewChild<T>(NewNodeUniqName);
			if(z.UniqName_TestNode.ContainsKey(node.UniqName)){
				throw new InvalidOperationException($"Duplicated test node uniq name: {node.UniqName}");
			}
			z.UniqName_TestNode.Add(node.UniqName, [node]);
			z.TesterType_TestNode.Add(typeof(T), [node]);
			t.RegisterTestsInto(node);
			IndexTesteeForSubtree(z, node);
			return NIL;
		});
	}

	public static partial void RegisterSubMgr(this IDiEtTestMgr z, IDiEtTestMgr SubMgr){
		var oldRoot = SubMgr.TestNode;
		var mountedRoot = z.TestNode.NewChild();
		mountedRoot.Ordered = oldRoot.Ordered;
		mountedRoot.IsParallelRecursive = oldRoot.IsParallelRecursive;
		SubMgr.TestNode = mountedRoot;
		z.DiFns.AddRange(SubMgr.DiFns);
		foreach(var subFn in SubMgr.RegisterTestFns){
			z.RegisterTestFns.Add((SvcP, _)=>subFn(SvcP, SubMgr.TestNode));
		}
	}
	
	public static partial IServiceProvider InitSvc(
		this IDiEtTestMgr z
		,IServiceCollection SvcColct
		,IServiceProvider SvcProvdr
	){
		_ = SvcColct;
		foreach(var fn in z.RegisterTestFns){
			fn(SvcProvdr, z.TestNode);
		}
		return SvcProvdr;
	}

	public static partial IServiceProvider InitSvc(
		this IDiEtTestMgr z
		,IServiceCollection SvcColct
		,Func<IServiceCollection, IServiceProvider> BuildSvcProvider
	){
		foreach(var fn in z.DiFns){
			fn(SvcColct);
		}
		IServiceProvider SvcProvdr = BuildSvcProvider(SvcColct);
		return z.InitSvc(SvcColct, SvcProvdr);
	}
	
	public static partial ITestNode GetNodeByName(this IDiEtTestMgr z, str UniqName){
		if(!z.UniqName_TestNode.TryGetValue(UniqName, out var nodes) || nodes.Count < 1){
			throw new KeyNotFoundException($"Test node not found by uniq name: {UniqName}");
		}
		return nodes[0];
	}
	
	public static partial ITestNode GetNodeByTesterType(this IDiEtTestMgr z, Type TesterType){
		if(!z.TesterType_TestNode.TryGetValue(TesterType, out var nodes) || nodes.Count < 1){
			throw new KeyNotFoundException($"Test node not found by tester type: {TesterType}");
		}
		return nodes[0];
	}
	
	public static partial ITestNode GetNodeByTesteeType(this IDiEtTestMgr z, Type TesteeType){
		if(!z.TesteeType_TestNode.TryGetValue(TesteeType, out var nodes) || nodes.Count < 1){
			throw new KeyNotFoundException($"Test node not found by testee type: {TesteeType}");
		}
		return nodes[0];
	}
	
	public static partial ITestNode GetNodeByTesteeFnName(this IDiEtTestMgr z, str TesteeFnName){
		if(!z.TesteeFnName_TestNode.TryGetValue(TesteeFnName, out var nodes) || nodes.Count < 1){
			throw new KeyNotFoundException($"Test node not found by testee fn name: {TesteeFnName}");
		}
		return nodes[0];
	}
	
	public static partial IList<ITestNode> GetNodesByTesteeType(this IDiEtTestMgr z, Type TesteeType){
		if(!z.TesteeType_TestNode.TryGetValue(TesteeType, out var nodes) || nodes.Count < 1){
			throw new KeyNotFoundException($"Test node not found by testee type: {TesteeType}");
		}
		return nodes;
	}
	
	public static partial IList<ITestNode> GetNodesByTesteeFnName(this IDiEtTestMgr z, str TesteeFnName){
		if(!z.TesteeFnName_TestNode.TryGetValue(TesteeFnName, out var nodes) || nodes.Count < 1){
			throw new KeyNotFoundException($"Test node not found by testee fn name: {TesteeFnName}");
		}
		return nodes;
	}

	private static partial void IndexTesteeForSubtree(IDiEtTestMgr z, ITestNode Root){
		foreach(var node in WalkNodes(Root)){
			var data = node.Data;
			if(data is null){
				continue;
			}
			if(data.TesteeTypes is not null){
				foreach(var t in data.TesteeTypes){
					if(t is null){
						continue;
					}
					AddNodeToDict(z.TesteeType_TestNode, t, node);
				}
			}
			if(data.TesteeFnNames is not null){
				foreach(var n in data.TesteeFnNames){
					if(string.IsNullOrWhiteSpace(n)){
						continue;
					}
					AddNodeToDict(z.TesteeFnName_TestNode, n, node);
				}
			}
		}
	}
	
	private static partial IEnumerable<ITestNode> WalkNodes(ITestNode Root){
		var stack = new Stack<ITestNode>();
		stack.Push(Root);
		while(stack.Count > 0){
			var cur = stack.Pop();
			yield return cur;
			if(cur.Children is null){
				continue;
			}
			for(int i = cur.Children.Count - 1; i >= 0; i--){
				stack.Push(cur.Children[i]);
			}
		}
	}
	
	private static partial void AddNodeToDict<TKey>(
		IDictionary<TKey, IList<ITestNode>> Dict
		,TKey Key
		,ITestNode Node
	) where TKey : notnull{
		if(!Dict.TryGetValue(Key, out var list)){
			list = [];
			Dict.Add(Key, list);
		}
		if(!list.Contains(Node)){
			list.Add(Node);
		}
	}
}