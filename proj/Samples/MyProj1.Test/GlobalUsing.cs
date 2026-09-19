// 檔案頭部：全局 using。
//
// `global using static MyProj1.Test.Di;`
// 讓各測試類直接寫 GetRSvc<T>()，
// 不必每處都帶 Di. 前綴。
//
// 這與其他測試工程的寫法一致
// （例如 Tsinswreng.CsSql.Test/GlobalUsing.cs）。
global using static MyProj1.Test.Di;