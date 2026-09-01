// Polyfill: 使 net461 目标支持 init / record 等 C# 9+ 语言特性
// 参考: https://learn.microsoft.com/dotnet/csharp/language-reference/compiler-messages/init-assign
#if NETFRAMEWORK

using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit
    {
    }
}

#endif
