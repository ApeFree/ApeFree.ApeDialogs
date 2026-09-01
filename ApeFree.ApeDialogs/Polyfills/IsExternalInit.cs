// Polyfill: 使 netstandard2.0 目标支持 init / record 等 C# 9+ 语言特性
// 参考官方文档: https://learn.microsoft.com/dotnet/csharp/language-reference/compiler-messages/init-assign
#if !NET5_0_OR_GREATER

using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit
    {
    }
}

#endif
