using System;

namespace MakeSharp
{
    public interface IConfigureTask
    {
        IDependOn Always { get; }
        ITaskAction When(Func<IScriptParams, bool> condition);
    }
}