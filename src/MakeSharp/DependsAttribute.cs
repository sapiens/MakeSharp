using System;

namespace MakeSharp
{
    public class DependsAttribute : Attribute
    {
        private readonly string[] _taskNames;

        public DependsAttribute(params string[] taskName)
        {
            _taskNames = taskName;
        }

        public string[] TaskNames
        {
            get { return _taskNames; }
        }
    }
}