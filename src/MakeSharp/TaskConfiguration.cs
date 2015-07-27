using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MakeSharp
{
    public class TaskConfiguration : IConfigureTask
    {
        private readonly Type _type;
        
        public TaskConfiguration(Type type)
        {
            _type = type;  
            FillAttributeDependencies(); 
            IsDefault = _type.HasCustomAttribute<DefaultAttribute>();   
        }

        public object Instance { get; set; }

        void FillAttributeDependencies()
        {
            var attrib = Type.GetCustomAttribute<DependsAttribute>();
            if (attrib==null) return;
            var asm = Type.Assembly;
            
            foreach (var typeName in attrib.TaskNames)
            {
                var type = asm.GetType(Type.GetNestedNameFor(typeName));
                var config = new TaskConditionalConfiguration(this);
                config.AddDependency(type);
                _configurations.Add(config);
            }
        }

       
        public bool IsDefault { get; private set; }

        public bool ShouldExecute(IScriptParams init)
        {
            var dont = _configurations.FirstOrDefault(d => d.AppliesTo(init) && !d.ShouldExecute);
            return dont == null;
        }

        public Type[] GetDependencies(IScriptParams init)
        {
            return _configurations.Where(c => c.AppliesTo(init)).SelectMany(c => c.Dependencies).ToArray();
        }

        List<TaskConditionalConfiguration> _configurations=new List<TaskConditionalConfiguration>();

        public IDependOn Always
        {
            get { return When(null); }
        }

        public Type Type
        {
            get { return _type; }
        }

        public ITaskAction When(Func<IScriptParams, bool> condition)
        {
            var config = new TaskConditionalConfiguration(this, condition);
            _configurations.Add(config);
            return config;
        }

        
    }
}