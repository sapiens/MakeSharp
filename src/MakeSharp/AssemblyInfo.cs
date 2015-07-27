using System.Linq;

namespace MakeSharp
{
    public class AssemblyInfo
    {
        public string Version { get; set; }

        public string FileVersion { get; set; }

        internal static string[] GetNames()
        {
            return typeof (AssemblyInfo).GetProperties().Select(d => d.Name).ToArray();
        }

        internal static string GetInternalPrefix(string name)
        {
            return "[assembly: Assembly" + name;
        }

        internal void SetValue(string property, string value)
        {
            this.GetType().GetProperty(property).SetValue(this,value);
        }
    }
}