using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

[Doc($$"""
用例函數。統一 async，同步邏輯直接返回 `NIL` 即可。

#Prm[{{nameof(Arg)}}][框架傳進來的參數，即 `{{nameof(OptTestExecutor.Arg)}}`，所有用例共用同一個引用]
""")]
public delegate Task<obj?> FnTest(obj? Arg);
