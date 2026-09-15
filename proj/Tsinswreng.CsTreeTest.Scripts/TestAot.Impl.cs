namespace Tsinswreng.CsTreeTest.Scripts;

using Tsinswreng.CsSh;
using static Tsinswreng.CsSh.ShGlobal;

internal static partial class TestAot{
	internal static async partial Task Main(ISCtx Ctx, CT Ct){
		var TestProject = Ctx.RootDir/"proj/Tsinswreng.CsTreeTest.Test/Tsinswreng.CsTreeTest.Test.csproj";
		var Rid = GetEnv("AOT_TEST_RID");
		if(String.IsNullOrWhiteSpace(Rid)){
			Rid = OperatingSystem.IsWindows() ? "win-x64" : "linux-x64";
		}
		await Exe("dotnet", ["publish", TestProject, "--configuration", "Release", "--runtime", Rid], Ct);

		var PublishDir = Ctx.RootDir/"proj/Tsinswreng.CsTreeTest.Test/bin/Release"/Ctx.Tfm/Rid/"publish";
		var ExecutableName = OperatingSystem.IsWindows() ? "Tsinswreng.CsTreeTest.Test.exe" : "Tsinswreng.CsTreeTest.Test";
		await Exe(PublishDir/ExecutableName, [], Ct);
	}
}