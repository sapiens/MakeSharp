using System.Collections.Generic;

namespace MakeSharp
{
    public interface IScriptParams
    {
        /// <summary>
        /// Gets the script's arguments in raw form
        /// </summary>
        IDictionary<int,string> RawArguments { get; }
        /// <summary>
        /// gets the script's arguments as a dynamic object
        /// </summary>
        dynamic Args { get; }
        T Get<T>(string name, T defaultValue = default(T));

        bool HasArgument(string name);
    }
}