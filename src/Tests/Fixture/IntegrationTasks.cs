using System.Text;
using FluentAssertions;
using MakeSharp;

namespace Tests.Fixture
{
    public class IntegrationTasks
    {
        public class Clean
        {
            public void Run()
            {
                Context.Data["Result"].As<StringBuilder>().Append("Clean");
            }

            public ITaskContext Context { get; set; }
        }
        
        [Depends("Clean")]
        [Default]
        public class Build
        {
            public void Run()
            {
                Context.Data["Result"].As<StringBuilder>().Append("Build");
            }

            public ITaskContext Context { get; set; }
        }

        public class Pack
        {
            public Pack(IConfigureTask cfg)
            {
                cfg.Always.DependOn<Build>();
            }
            public void Run()
            {
                Context.Data["Result"].As<StringBuilder>().Append("Pack");
            }

            public ITaskContext Context { get; set; }
        }
    }
}