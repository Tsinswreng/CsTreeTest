namespace Tsinswreng.CsTreeTest.Scripts;

using Tsinswreng.CsSh;
using static Tsinswreng.CsSh.ShGlobal;

internal static partial class Test{
	internal static async partial Task Main(ISCtx Ctx, CT Ct){
		await Exe("dotnet", ["run", "--project", Ctx.RootDir/"proj/Tsinswreng.CsTreeTest.Test/Tsinswreng.CsTreeTest.Test.csproj"], Ct);
	}
}