using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CavemanTools.Logging;

namespace MakeSharp
{
    public class AssemblyInfoFile
    {
        private string _file;

        public AssemblyInfoFile(string file,bool populate=true)
        {
            _file = file;
            if (populate) PopulateInfo();
        }

        public AssemblyInfo Info { get; set; }=new AssemblyInfo();

        public void PopulateInfo()
        {
            var file = File.ReadLines(_file);
            
            foreach (var name in AssemblyInfo.GetNames())
            {
                var line = file.FirstOrDefault(l => l.StartsWith(AssemblyInfo.GetInternalPrefix(name)));
                if (line == null) continue;
                Info.SetValue(name,ExtractValue(line));
            }
            EnsureSemanticVersions();
            
        }

        void EnsureSemanticVersions()
        {
            if (Info.Version.Count(d=>d=='.')!=3) return;
            var last = Info.Version.LastIndexOf('.');
            Info.Version = Info.FileVersion = Info.Version.Substring(0, last);
        }

        string ExtractValue(string source)
        {
            var idx = source.IndexOf('"');
            var last = source.IndexOf('"', idx + 1);
            return source.Substring(idx + 1, last - idx).TrimEnd('"');
        }

        public void Save()
        {
            var all =
                AssemblyInfo.GetNames()
                    .Select(n => new {key = n, value = Info.GetPropertyValue(n)})
                    .ToDictionary(d => d.key, d => d.value);
            Save(all);
        }


        private void Save(IDictionary<string,object> values)
        {
            
            var all = File.ReadAllText(_file);
            foreach (var kv in values)
            {
                var prefix = AssemblyInfo.GetInternalPrefix(kv.Key);
                var nv = prefix + "(\"" + kv.Value + "\")]";
                
                all = Regex.Replace(all, @"\" + prefix + @".*\]", nv,RegexOptions.Multiline);
            }
            File.WriteAllText(_file,all);
            
        }

    }
}