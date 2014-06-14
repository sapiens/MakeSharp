using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MakeSharp
{
    public class MakeSharpConfiguration:IConfigureMakeSharp
    {
        private Func<MethodInfo, bool> _matchMethod=m=>m.Name=="Run";
        private Func<Type, bool> _matchType=t=> !t.IsAbstract && t.IsClass && t.IsNestedPublic;

        public MakeSharpConfiguration()
        {
            ScriptArguments=new string[0];
        }

        public bool IsTask(Type type)
        {
            return type.IsNested && _matchType(type);
        }

        public MethodInfo GetExecutableMethod(Type t)
        {
            var m= t.GetMethods().FirstOrDefault(_matchMethod);
            if (m==null) throw new InvalidOperationException("Task '{0}' doesn't have a method matching the defined convention. The default method is 'Run'".ToFormat(t.Name));
            return m;
        }

        public TaskContext CreateContext(IScriptParams init)
        {
            Debug.Assert(Context==null);
            var context = new TaskContext(init);
            context.RequestedTask = ScriptName;
            Context = context;
            return context;
        }

        public TaskContext Context { get; private set; }


        public string ScriptName { get; set; }
        public string[] ScriptArguments { get; set; }

        public IConfigureMakeSharp TasksAre(Func<Type, bool> matchType)
        {
            matchType.MustNotBeNull();
            _matchType = matchType;
            return this;
        }

        public IConfigureMakeSharp MethodToExecute(Func<MethodInfo, bool> matchMethod)
        {
            matchMethod.MustNotBeNull();
            _matchMethod = matchMethod;
            return this;
        }
    }
}