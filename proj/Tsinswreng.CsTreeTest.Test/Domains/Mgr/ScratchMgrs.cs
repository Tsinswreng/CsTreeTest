using Tsinswreng.CsCore;
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsTreeTest.Test.Domains.Mgr;

// 分片聲明文件：本片聲明本領域各用例要用的臨時管理器；實現見 ScratchMgrs.Impl.cs。
[Doc($$"""
#Sum[本領域各用例要用的臨時管理器。]
#Descr[
全局那個管理器已經初始化過
（且執行期間並行跑用例，改它的索引不安全），
所以每個要動樹結構或索引的用例
都在自己新建的臨時管理器上操作。
]
""")]
public partial class TestMgrCase{
	[Doc($$"""
#Sum[帶一個測試類的管理器。]
#Descr[
`SetupTree` 只掛 `TestRegister` 一個測試類，
用例據此得到「只有一條測試類節點」的最小樹。
]
""")]
	private sealed partial class OneTesterMgr : TestMgr{
		[Doc($$"""
#Sum[本管理器的樹結構：註冊 `TestRegister` 一個測試類。]
""")]
		protected override partial void SetupTree();
	}

	[Doc($$"""
#Sum[把測試類註冊到領域分組下的管理器。]
""")]
	private sealed partial class BranchMgr : TestMgr{
		[Doc($$"""
#Sum[本管理器的樹結構：把 `TestAssert` 歸到名為 `Asserts` 的分組節點下。]
""")]
		protected override partial void SetupTree();
	}

	[Doc($$"""
#Sum[空的父管理器，用來收編子管理器。]
""")]
	private sealed partial class ParentMgr : TestMgr{
		[Doc($$"""
#Sum[本管理器的樹結構：空，本管理器只作為收編子管理器的父級。]
""")]
		protected override partial void SetupTree();
	}
}