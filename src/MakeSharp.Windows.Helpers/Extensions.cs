using System.Collections.Specialized;

namespace MakeSharp.Windows.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Invokes MSBuild.exe with Clean target and normal verbosity
        /// </summary>
        /// <param name="project">Project/solution file</param>
        /// <param name="configuration">Default is "Release"</param>
        public static void MsBuildClean(this string project, string configuration = "Release")
        {
            var builder = new MsBuild(project, new NameValueCollection() { { "Configuration", configuration } });
            builder.Clean();
        }

        /// <summary>
        /// Invokes MSBuild.exe with Build target, Release configuration and no verbosity
        /// </summary>
        /// <param name="project">Project/solution file</param>
        public static void MsBuildRelease(this string project)
        {
            var builder = new MsBuild(project, MsBuild.ConfigurationRelease);
            builder.Build();
        }

        /// <summary>
        /// Copies files from srcDist to destDir using robocopy.exe (it should be default on any Windows starting with Vista)
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="destDir"></param>
        public static void Robocopy(this string sourceDir, string destDir)
        {
            "robocopy".Exec(sourceDir, destDir, "/E", "/XN", "/NS", "/NC", "/NJH", "/NJS");
        }
    }
}