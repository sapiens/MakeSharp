using System;
using System.Collections.Generic;
using System.Linq;

namespace MakeSharp
{
    public class TasksList
    {
        private readonly MakeSharpConfiguration _configuration;

        public TasksList(MakeSharpConfiguration configuration)
        {
            _configuration = configuration;
        }

        public TaskConfiguration CreateConfiguration(Type type)
        {
            var config = new TaskConfiguration(type);
            
            _tasks.Add(config);

            return config;
        }
        List<TaskConfiguration> _tasks=new List<TaskConfiguration>();

        public TaskConfiguration GetConfiguration(Type type)
        {
            return _tasks.Find(t => t.Type == type);
        }

        public TaskExecutor BuildExecutor(TaskContext context)
        {
            var name = context.RequestedTask;
            TaskConfiguration task = null;
            if (name.IsNullOrEmpty())
            {
                task = _tasks.FirstOrDefault(d => d.IsDefault);
                if (task==null) throw new InvalidOperationException("There's no default task specified. Either specify a script name or annotate a task with [Default]");                
            }
            else
            {
                task = _tasks.Find(d => d.Type.Name == name);
                if (task==null) throw new InvalidOperationException("Can't find task '{0}'".ToFormat(name));
            }
            return GetExecutor(task,context);
        }

        TaskExecutor GetExecutor(TaskConfiguration config, TaskContext context)
        {
            var deps = config.GetDependencies(context.InitData);
            var execDeps = new TaskExecutor[deps.Length];
            for (var i = 0; i < deps.Length; i++)
            {
                var typeConfig = _tasks.Find(d => d.Type == deps[i]);
                if (typeConfig==null)throw new InvalidOperationException("Can't find task '{0}'".ToFormat(deps[i].Name));
                execDeps[i] = GetExecutor(typeConfig, context);
            }
            if (!config.ShouldExecute(context.InitData)) return TaskExecutor.SkipTask;
            var method = _configuration.GetExecutableMethod(config.Type);
            
            return new TaskExecutor(config.Instance,method,execDeps);
        }
    }
}