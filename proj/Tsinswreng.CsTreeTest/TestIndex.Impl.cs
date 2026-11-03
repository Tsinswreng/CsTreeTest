using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public partial class TestIndex{
	public partial void Add(ITestNode Node){
		Add(Node, Node is ITesterNode TesterNd ? TesterNd.TesterType : null);
	}

	public partial void Add(ITestNode Node, Type? TesterType){
		// step 1：唯一名這組。允許覆蓋同一個實例，撞名則直接失敗。
		if(Node.UniqName is {Length: > 0} Name){
			if(UniqName_TestNode.TryGetValue(Name, out var Old) && !ReferenceEquals(Old, Node)){
				throw new InvalidOperationException(
					$"重複的節點唯一名：{Name}"
				);
			}
			UniqName_TestNode[Name] = Node;
		}

		// step 2：測試類這組。一個測試類只能對應一個節點。
		if(TesterType is { } TT){
			if(TesterType_TestNode.TryGetValue(TT, out var Old) && !ReferenceEquals(Old, Node)){
				throw new InvalidOperationException(
					$"重複的測試類：{TT.FullName}"
				);
			}
			TesterType_TestNode[TT] = Node;
		}

		// step 3：用例元數據這兩組。
		// 元數據只掛在帶用例的葉節點上；
		// 分組節點的 Data 為空，
		// 因而不會進被測類型與被測函數名的索引。
		var Data = Node.Data;
		if(Data is null){
			return;
		}
		foreach(var TesteeType in Data.TesteeTypes){
			AddToDict(TesteeType_TestNode, TesteeType, Node);
		}
		foreach(var FnName in Data.TesteeFnNames){
			// 空白的函數名無意義，跳過以免污染索引、
			// 使之後按名字查到一堆不相關的節點。
			if(string.IsNullOrWhiteSpace(FnName)){
				continue;
			}
			AddToDict(TesteeFnName_TestNode, FnName, Node);
		}
	}

	public partial void MergeFrom(TestIndex Other, str? SkipUniqName){
		ArgumentNullException.ThrowIfNull(Other);
		// step 1：唯一名這組，逐條覆蓋；SkipUniqName 那條直接不放進來。
		foreach(var Pair in Other.UniqName_TestNode){
			if(Pair.Key == SkipUniqName){
				continue;
			}
			UniqName_TestNode[Pair.Key] = Pair.Value;
		}
		// step 2：測試類這組，一對一，直接覆蓋。
		foreach(var Pair in Other.TesterType_TestNode){
			TesterType_TestNode[Pair.Key] = Pair.Value;
		}
		// step 3：兩個一對多的組，按追加合併，已有的條目不動。
		foreach(var Pair in Other.TesteeType_TestNode){
			foreach(var Node in Pair.Value){
				AddToDict(TesteeType_TestNode, Pair.Key, Node);
			}
		}
		foreach(var Pair in Other.TesteeFnName_TestNode){
			foreach(var Node in Pair.Value){
				AddToDict(TesteeFnName_TestNode, Pair.Key, Node);
			}
		}
	}

	public partial ITestNode? FindByName(str UniqName){
		return UniqName_TestNode.TryGetValue(UniqName, out var Node) ? Node : null;
	}

	public partial ITestNode? FindByTesterType(Type TesterType){
		return TesterType_TestNode.TryGetValue(TesterType, out var Node) ? Node : null;
	}

	public partial IList<ITestNode> FindByTesteeType(Type TesteeType){
		return TesteeType_TestNode.TryGetValue(TesteeType, out var Nodes) ? Nodes : [];
	}

	public partial IList<ITestNode> FindByTesteeFnName(str TesteeFnName){
		return TesteeFnName_TestNode.TryGetValue(TesteeFnName, out var Nodes) ? Nodes : [];
	}

	private static partial void AddToDict<TKey>(
		IDictionary<TKey, IList<ITestNode>> Dict
		,TKey Key
		,ITestNode Node
	) where TKey:notnull{
		if(!Dict.TryGetValue(Key, out var List)){
			List = [];
			Dict[Key] = List;
		}
		if(!List.Contains(Node)){
			List.Add(Node);
		}
	}
}
