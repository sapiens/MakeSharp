using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace MakeSharp
{
    public class DefaultInitObject:IScriptParams
    {
        private IDictionary<int, string> _rawArguments;
        //private Dictionary<string, string> _params=null;
        dynamic _params = null;

        public DefaultInitObject()
        {
            RawArguments=new Dictionary<int, string>();

        }

        public IDictionary<int, string> RawArguments
        {
            get { return _rawArguments; }
            set
            {
                _rawArguments = value;
               
            }
        }

        private IDictionary<string, object> _Params
        {
            get
            {
                if (_params==null) ExtractParams();
                return _params;
            }
        }

        public dynamic Args
        {
            get { return _params; }
        }

        private void ExtractParams()
        {
            _params=new ExpandoObject();
            var d = _params as IDictionary<string, object>;
            
            RawArguments.Values.ForEach(p =>
            {
                var kv = p.Split('=');
                d[kv[0]] = (kv.Length == 2 ? kv[1].Trim('"') : "1");

            });
        }

        public T Get<T>(string name, T defaultValue = default(T))
        {
            var val= _Params.GetValueOrDefault(name);
            if (val==null) return defaultValue;
            return val.ConvertTo<T>();
        }

        public bool HasArgument(string name)
        {
            return _Params.ContainsKey(name);
        }
    }
}