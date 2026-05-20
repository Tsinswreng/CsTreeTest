namespace Tsinswreng.CsTreeTest;

public class Assert{
	public static nil IsTrue(bool Cond, str? ErrMsg = null){
		if(!Cond){
			throw new Exception(ErrMsg??"");
		}
		return NIL;
	}
}
