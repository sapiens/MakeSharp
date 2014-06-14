using System;
using System.Collections.Generic;

namespace MakeSharp
{
    public class TaskContext : ITaskContext
    {
      public TaskContext(IScriptParams init)
        {
            init.MustNotBeNull();
            InitData = init;
            Data=new Dictionary<string, object>();
            Timer = new TaskTimer();
        }

        public TaskTimer Timer { get; private set; }
        public IScriptParams InitData { get; private set; }
        public string RequestedTask { get; set; }
        public IDictionary<string, object> Data { get; private set; }
    }
}