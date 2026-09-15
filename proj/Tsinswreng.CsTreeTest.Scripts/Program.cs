namespace Tsinswreng.CsTreeTest.Scripts;

using System.Runtime.CompilerServices;
using Tsinswreng.CsSh;
using static Tsinswreng.CsSh.ShGlobal;

/// 子命令共用的執行上下文。
/// dispatcher 統一建立並填入倉庫固定事實，子命令直接取用、不各自解析路徑。
public interface ISCtx{
	/// 倉庫根目錄，各命令以此定位 proj/ 下的專案。
	public Pth RootDir{get;set;}
	/// 目標框架（TFM），與 proj/Directory.Build.props 的 TargetFramework 同步；
	/// 決定構建/發布輸出目錄名（如 net10.0）。升級 TFM 時此處需同步更新。
	public str Tfm{get;set;}
}

public class SCtx:ISCtx{
	public Pth RootDir{get;set;} = "";
	public str Tfm{get;set;} = "";
}

/// CsTreeTest 維護腳本的命令行入口。
/// 用法：dotnet run --project proj/Tsinswreng.CsTreeTest.Scripts -- &lt;Test|TestAot|Release&gt;
internal static partial class Program{
	internal static async Task Main(str[] Args){
		var Ct = default(CT);
		var Root = FullPath(DirName(OwnPath())/"../..");
		ISCtx Ctx = new SCtx{RootDir = Root, Tfm = "net10.0"};
		if(Args.Length == 0){ PrintUsage(); return; }
		switch(Args[0]){
			case nameof(Test): await Test.Main(Ctx, Ct); break;
			case nameof(TestAot): await TestAot.Main(Ctx, Ct); break;
			case nameof(Release): await Release.Main(Ctx, Ct); break;
			default: throw new ArgumentException($"Unknown CsTreeTest script: {Args[0]}.", nameof(Args));
		}
	}

	private static void PrintUsage(){
		Console.Error.WriteLine("Usage: dotnet run --project proj/Tsinswreng.CsTreeTest.Scripts -- <entry>");
		Console.Error.WriteLine("Entries: Test, TestAot, Release");
	}

	/// 本文件路徑（編譯期常量），用於定位倉庫根目錄，而不是依賴運行時 cwd。
	private static str OwnPath([CallerFilePath] str CallerPath = ""){ return CallerPath; }
}