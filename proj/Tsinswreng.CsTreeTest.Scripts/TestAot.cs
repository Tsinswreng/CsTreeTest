namespace Tsinswreng.CsTreeTest.Scripts;

using Tsinswreng.CsSh;

/// 以 Release + NativeAOT 發布測試工程並運行發布產物，驗證可 AOT 發布。
/// RID 可由環境變量 AOT_TEST_RID 覆蓋，默認取當前 OS（win-x64/linux-x64）。
internal static partial class TestAot{
	internal static partial Task Main(ISCtx Ctx, CT Ct);
}