using System.Collections.Generic;

namespace MakeSharp
{
    public interface ITaskContext
    {
        IScriptParams InitData { get; }
        IDictionary<string, object> Data { get; }
    }
}