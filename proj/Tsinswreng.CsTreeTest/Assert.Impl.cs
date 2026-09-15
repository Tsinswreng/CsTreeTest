namespace Tsinswreng.CsTreeTest;

// 實現文件：只放函數實現。
public partial class Assert{
	public static partial nil IsTrue(bool Cond, str? ErrMsg){
		if(!Cond){
			throw new Exception(ErrMsg??"");
		}
		return NIL;
	}
}