using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace MakeSharp
{
    public class TaskConditionalConfiguration:ITaskAction
    {
        private readonly TaskConfiguration _parent;
        private readonly Func<IScriptParams, bool> _condition=s=>true;
        private List<Type> _dependencies= new List<Type>();

        public TaskConditionalConfiguration(TaskConfiguration parent, Func<IScriptParams, bool> condition=null)
        {
            _parent = parent;
            if (condition!=null) _condition = condition;
            ShouldExecute = true;
            
        }

        public bool AppliesTo(IScriptParams context)
        {
            return _condition(context);
        }
        public bool ShouldExecute { get; private set; }

        public Type[] Dependencies
        {
            get { return _dependencies.ToArray(); }
            
        }

        public void AddDependency(Type type)
        {
            if (_dependencies.Any(d => d == type)) return;            
            _dependencies.Add(type);
        }

        public IDependOn DependOn<T>()
        {
           AddDependency(typeof(T));
            return this;
        }

        public IConfigureTask But
        {
            get { return _parent; }
        }
        public void DontExecute()
        {
            ShouldExecute = false;
        }
    }
}