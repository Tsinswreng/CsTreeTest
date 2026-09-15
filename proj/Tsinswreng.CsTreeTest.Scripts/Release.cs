namespace Tsinswreng.CsTreeTest.Scripts;

using Tsinswreng.CsSh;

/// 發布流程：驗證 → 解析標籤 → 打包 → 推 NuGet → 發 GitHub Release。
/// 無 v* 標籤時只做驗證（CI 的 PR/推 master 場景）。
internal static partial class Release{
	internal static partial Task Main(ISCtx Ctx, CT Ct);

	/// 驗證：AOT 發布並運行測試 + Release 構建庫。
	private static partial Task<nil> Verify(ISCtx Ctx, CT Ct);

	/// 打包庫到 artifacts/release，返回 nupkg/snupkg 列表。
	private static partial Task<IReadOnlyList<Pth>> Pack(ISCtx Ctx, str Version, CT Ct);

	/// 推送 NuGet（先 nupkg 後 snupkg，--skip-duplicate）。
	private static partial Task<nil> PushNuGet(IReadOnlyList<Pth> Packages, CT Ct);

	/// 發布/更新 GitHub Release：存在則 --clobber 上傳，否則新建。
	private static partial Task<nil> PublishGitHubRelease(str Tag, IReadOnlyList<Pth> Packages, CT Ct);

	/// 從 GITHUB_REF_NAME/GITHUB_REF_TYPE 解析發布標籤；非標籤環境返回 null。
	private static partial str? ResolveTag();

	/// 校驗標籤格式並剝掉前導 v（^v\d+\.\d+\.\d+(-...)?$）。
	private static partial str ParseVersion(str Tag);
}