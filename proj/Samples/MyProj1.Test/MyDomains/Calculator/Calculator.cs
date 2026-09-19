namespace MyProj1.Test.MyDomains.Calculator;

/// 被測接口。
/// 實驗示例把被測類放在測試工程裏，
/// 實際項目中被測類在別的 csproj。
public interface ICalculator{
	/// 兩數相加。
	int Add(int A, int B);

	/// 整數除法。
	/// 除數為零時返回 false，
	/// 不出異常。
	bool TryIntDivide(int A, int B, out int R);
}

/// 被測類的默認實現。
public class Calculator : ICalculator{
	public int Add(int A, int B){
		return A + B;
	}

	public bool TryIntDivide(int A, int B, out int R){
		if(B == 0){
			R = default;
			return false;
		}
		R = A / B;
		return true;
	}
}