using System;
using System.IO;

namespace MakeSharp
{
    public class BuildScript
    {
        public static string Directory
        {
            get { return Environment.CurrentDirectory; }
        }

     
        public static string TempDirectory
        {
            get { return Path.Combine(Directory,TempDirName); }
        }

        public static string TempDirName = @"temp";

        /// <summary>
        /// File should be in the same folder as the script with name [project_name].nuspec
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public static NuSpecFile GetNuspecFile(string projectName)
        {
            return Path.Combine(Directory, projectName + ".nuspec").AsNuspec();
        }

        /// <summary>
        /// Creates a temp dir for the project and returns the path
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static string CreateTempDirectory(Project project)
        {
            var directory = GetProjectTempDirectory(project);
            directory.MkDir();
            return directory;
        }

        public static string GetProjectTempDirectory(Project project)
        {
            return Path.Combine(TempDirectory, project.Name);
        }

        //public static string PackageDir = @"temp/package";
    }
}