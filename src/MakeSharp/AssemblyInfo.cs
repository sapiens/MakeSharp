using System.Linq;
using CavemanTools;

namespace MakeSharp
{
    public class AssemblyInfo
    {
        public string Version { get; set; }

        public string FileVersion { get; set; }

        public void BumpMinorVersion()
        {
            var semver=new SemanticVersion(Version);
            var newVer=new SemanticVersion(semver.Major,semver.Minor+1,build:null,preRelease:null);
            Version = FileVersion = newVer.ToString();
        }

        public void BumpPatchVersion()
        {
           
            var semver = new SemanticVersion(Version);
            var newVer = new SemanticVersion(semver.Major, semver.Minor, semver.Patch+1,build:null, preRelease: null);
            Version = FileVersion = newVer.ToString();
        }

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