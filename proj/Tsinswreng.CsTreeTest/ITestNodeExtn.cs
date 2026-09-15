using Tsinswreng.CsCore;
using Tsinswreng.CsU128Id;

namespace Tsinswreng.CsTreeTest;

// ITestNode 的擴展：掛子節點 / 建註冊器。
// 聲明文件：傳統 this 擴展（調用方寫法與 extension 塊一致）+ partial method 拆分；實現見 ExtnITestNode.Impl.cs。
public static partial class ITestNodeExtn{
	/// Make a new child of self（普通子節點）。
	public static partial ITestNode NewChild(this ITestNode z, str? UniqName = null);

	/// Make a new child of self（TesterNode，帶 TesterType）。
	public static partial ITestNode NewChild<T>(this ITestNode z, str? UniqName = null);

	[Doc(@$"Make a function to register `{nameof(FnTest)}`.
	this can help you simplify your code
	,without duplicately passing the same `{nameof(Type)}`
	")]
	[Obsolete(@$"use {nameof(MkTestFnRegister)}")]
	public static partial Func<str, FnTest, nil> MkFnRegisterTest(
		this ITestNode z
		,Type TesterType
		,Type TesteeType
		,str UniqNamePrefix = ""
	);

	/// 建註冊器：tester type 顯式傳、testee 元數據一次傳入。
	public static partial ITestFnRegister MkTestFnRegister(
		this ITestNode z
		,Type TesterType
		,IList<Type> TesteeTypes
		,IList<str> TesteeFnNames
		,str UniqNamePrefix = ""
	);

	[Doc(@$"Same as {nameof(MkTestFnRegister)} but tester type is inferred from the
	generic parameter `TTester`; testee types / fn names are optional (default empty,
	they are only used for the future filtering indexes)
	")]
	public static partial ITestFnRegister MkTestFnRegister<TTester>(
		this ITestNode z
		,IList<Type>? TesteeTypes = null
		,IList<str>? TesteeFnNames = null
		,str UniqNamePrefix = ""
	);
}
