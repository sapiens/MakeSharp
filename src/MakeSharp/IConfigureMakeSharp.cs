using System;
using System.Reflection;

namespace MakeSharp
{
    public interface IConfigureMakeSharp
    {
        IConfigureMakeSharp TasksAre(Func<Type, bool> matchType);
        IConfigureMakeSharp MethodToExecute(Func<MethodInfo, bool> matchMethod);
    }
}