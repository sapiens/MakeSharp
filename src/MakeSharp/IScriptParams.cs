using System.Collections.Generic;

namespace MakeSharp
{
    public interface IScriptParams
    {
        IDictionary<int,string> ScriptParams { get; set; }
    }
}