using System;
using System.Linq;

namespace MakeSharp
{
    public static class Extensions
    {
        public static string GetNestedNameFor(this Type type, string name)
        {
            return type.FullName.Replace(type.Name, name);
        }

        
        public static bool HasValue(this IScriptParams init, string value)
        {
            return init.RawArguments.Values.Any(d => d == value);
        }

        
    }
}