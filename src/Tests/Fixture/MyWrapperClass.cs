using System;
using System.Linq;
using MakeSharp;

namespace Tests.Fixture
{
    public class MyWrapperClass
    {
        public class Clean
        {
            public void Execute()
            {
                Context.Data["Result"] = 23;
            }

            public ITaskContext Context { get; set; }
        }


        public class Build
        {
            public Build(IConfigureTask cfg)
            {
                cfg.Always
                    .DependOn<Clean>()
                    .DependOn<Dummy>();

                cfg.When(p => p.HasValue("skip"))
                    .DontExecute();
            }

            public void Execute() { }

        }

        [Depends("Clean")]
        public class Dummy
        {
            public void Execute()
            {
                var val=Context.Data["Result"].Cast<int>();
                val+= 10;
                Context.Data["Result"] = val;
            }

            public ITaskContext Context { get; set; } 
        }

        [Depends("Build","Dummy")]
        public class Pack {}
    }
}