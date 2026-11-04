using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

[Doc($$"""
最簡斷言：條件不成立就拋異常。用例拋異常即算失敗，所以不必用返回值表示成敗。
""")]
public partial class Assert{
	[Doc($$"""
斷言成立；不成立就拋帶 `{{nameof(ErrMsg)}}` 的 `Exception`。

#Prm[{{nameof(ErrMsg)}}][失敗時的異常消息；建議寫清期望值與實際值，為空時消息也是空字符串]
#Rtn[`NIL`；成功時只表示沒拋異常，不用接]
""")]
	public static partial nil IsTrue(bool Cond, str? ErrMsg = null);
}
