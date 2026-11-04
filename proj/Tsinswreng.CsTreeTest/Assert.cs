using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

public partial class Assert{
	[Doc($$"""
#Prm[{{nameof(ErrMsg)}}][斷言失敗時拋出的異常的ErrMsg]
""")]
	public static partial nil IsTrue(bool Cond, str? ErrMsg = null);
}
