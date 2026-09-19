using Tsinswreng.CsCore;

namespace Tsinswreng.CsTreeTest;

[Doc(@$"Function to test
Designed as async function to be compatible for both sync and async code.
#Params([Any])
#Rtn[Any]
")]
public delegate Task<obj?> FnTest(obj? Arg);
