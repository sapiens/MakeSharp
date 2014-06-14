using System;
using MakeSharp;

namespace Tests
{
    public class Setup
    {
        public static IScriptParams FakeScriptParamsObject()
        {
            return new DefaultInitObject();
        }

        public static TaskContext Context()
        {
            var c = new TaskContext(FakeScriptParamsObject());
            return c;
        }

        public static MakeSharpConfiguration Configuration(Action<MakeSharpConfiguration> config=null)
        {
            var res=new MakeSharpConfiguration();
            config.IfNotNullDo(d=>d(res));
            return res;
        }
    }
}