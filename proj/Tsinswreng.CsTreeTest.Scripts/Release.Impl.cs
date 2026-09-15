namespace Tsinswreng.CsTreeTest.Scripts;

using System.Text.RegularExpressions;
using Tsinswreng.CsSh;
using static Tsinswreng.CsSh.ShGlobal;

internal static partial class Release{
	internal static async partial Task Main(ISCtx Ctx, CT Ct){
		await Verify(Ctx, Ct);
		var Tag = ResolveTag();
		if(Tag is null){
			Echo("Verification succeeded; no release tag was supplied.");
			return;
		}
		var Version = ParseVersion(Tag);
		var Packages = await Pack(Ctx, Version, Ct);
		await PushNuGet(Packages, Ct);
		await PublishGitHubRelease(Tag, Packages, Ct);
	}

	private static async partial Task<nil> Verify(ISCtx Ctx, CT Ct){
		// 1) AOT 發布測試並運行（測試失敗會讓進程非零退出，Exe 拋錯）
		await TestAot.Main(Ctx, Ct);
		// 2) Release 構建庫本體
		var PackageProject = Ctx.RootDir/"proj/Tsinswreng.CsTreeTest/Tsinswreng.CsTreeTest.csproj";
		await Exe("dotnet", ["build", PackageProject, "--configuration", "Release", "--no-restore"], Ct);
		return NIL;
	}

	private static async partial Task<IReadOnlyList<Pth>> Pack(ISCtx Ctx, str Version, CT Ct){
		var ArtifactDir = Ctx.RootDir/"artifacts/release";
		if(await Exists(ArtifactDir, Ct)){ await Rm(ArtifactDir, Ct); }
		await Mkdir(ArtifactDir, Ct);
		var PackageProject = Ctx.RootDir/"proj/Tsinswreng.CsTreeTest/Tsinswreng.CsTreeTest.csproj";
		await Exe("dotnet", ["pack", PackageProject, "--configuration", "Release", "--no-build", "--no-restore", "--output", ArtifactDir, $"-p:PackageVersion={Version}"], Ct);

		var Packages = new List<Pth>();
		foreach(var FilePath in LsFile(ArtifactDir)){
			var Name = FilePath.ToString();
			if(Name.EndsWith(".nupkg", StringComparison.OrdinalIgnoreCase) || Name.EndsWith(".snupkg", StringComparison.OrdinalIgnoreCase)){
				Packages.Add(FilePath);
			}
		}
		if(Packages.Count == 0){
			throw new InvalidOperationException($"dotnet pack produced no NuGet artifacts in {ArtifactDir}.");
		}
		return Packages;
	}

	private static async partial Task<nil> PushNuGet(IReadOnlyList<Pth> Packages, CT Ct){
		var ApiKey = GetEnv("NUGET_API_KEY");
		if(String.IsNullOrWhiteSpace(ApiKey)){
			throw new InvalidOperationException("NUGET_API_KEY is required for a tagged release.");
		}
		const str NuGetSource = "https://api.nuget.org/v3/index.json";
		foreach(var Package in Packages.OrderBy(Package => Package.ToString().EndsWith(".snupkg", StringComparison.OrdinalIgnoreCase))){
			await Exe("dotnet", ["nuget", "push", Package, "--source", NuGetSource, "--api-key", ApiKey, "--skip-duplicate"], Ct);
		}
		return NIL;
	}

	private static async partial Task<nil> PublishGitHubRelease(str Tag, IReadOnlyList<Pth> Packages, CT Ct){
		var ViewResult = await TryExe("gh", ["release", "view", Tag, "--json", "tagName"], Ct);
		if(ViewResult.IsSuccess){
			var UploadArgs = new List<str>{"release", "upload", Tag, "--clobber"};
			foreach(var Package in Packages){ UploadArgs.Add(Package); }
			await Exe("gh", UploadArgs, Ct);
			return NIL;
		}
		var CreateArgs = new List<str>{"release", "create", Tag, "--verify-tag", "--generate-notes", "--title", Tag};
		foreach(var Package in Packages){ CreateArgs.Add(Package); }
		await Exe("gh", CreateArgs, Ct);
		return NIL;
	}

	private static partial str? ResolveTag(){
		var EnvironmentTag = GetEnv("GITHUB_REF_NAME");
		if(String.Equals(GetEnv("GITHUB_REF_TYPE"), "tag", StringComparison.OrdinalIgnoreCase) && !String.IsNullOrWhiteSpace(EnvironmentTag)){
			return EnvironmentTag;
		}
		return null;
	}

	private static partial str ParseVersion(str Tag){
		if(!Regex.IsMatch(Tag, "^v[0-9]+\\.[0-9]+\\.[0-9]+(-[0-9A-Za-z][0-9A-Za-z.-]*)?$", RegexOptions.CultureInvariant)){
			throw new ArgumentException("Release tag must match vMAJOR.MINOR.PATCH with an optional prerelease suffix.", nameof(Tag));
		}
		return Tag[1..];
	}
}