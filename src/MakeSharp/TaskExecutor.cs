using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MakeSharp
{
    public class TaskExecutor
    {
        private readonly object _instance;
        private readonly MethodInfo _method;
        private TaskExecutor[] _deps=new TaskExecutor[0];

        public static TaskExecutor SkipTask=new TaskExecutor();

        private TaskExecutor()
        {
            
        }

        public TaskExecutor(object instance, MethodInfo method,params TaskExecutor[] deps)
        {
            _instance = instance;
            _method = method;

            if(deps.HasItems())_deps = deps;
        }

        public Type Type
        {
            get { return _instance.GetType(); }
        }

        /// <summary>
        /// Returns dependencies as types
        /// </summary>
        public Type[] TaskDependencies
        {
            get { return _deps.Select(d => d._instance.GetType()).ToArray(); }
        }

        public TaskExecutor[] Dependencies
        {
            get { return _deps; }           
        }

        public void Execute(TaskContext context)
        {
            if (_instance == null)
            {
                return;
            }

            foreach (var task in Dependencies)
            {
                task.Execute(context);
            }
            
            //var method = _instance.GetType().GetMethods().FirstOrDefault(_matchMethod);
            //if (method==null) throw new InvalidOperationException("No method in '{0}' matches convention to execute".ToFormat(_instance.GetType()));
            var name = _instance.GetType().Name;
            "Executing {0}".WriteInfo(name);
            context.Timer.Start(name);
            _method.Invoke(_instance,null);
            context.Timer.End();
        }

       

    }
}