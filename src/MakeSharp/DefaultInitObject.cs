using System.Collections.Generic;

namespace MakeSharp
{
    public class DefaultInitObject:IScriptParams
    {
        public DefaultInitObject()
        {
            ScriptParams=new Dictionary<int, string>();
        }
        public IDictionary<int, string> ScriptParams { get; set; }
    }
}