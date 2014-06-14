using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
    }

    public class Solution
    {
        public static string FileName;
        private static Solution _instance;

        public static Solution Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Solution(FileName);
                }
                return _instance;
            }
        }

        public Solution(string filename)
        {
            filename.MustNotBeEmpty();
            filename.MustComplyWith(fi=>fi.EndsWith(".sln"),"A solution file ends with .sln");
            var f = new FileInfo(filename);
            Directory = f.Directory.FullName;
            FilePath = filename;
            Projects=new List<Project>();
        }

        public string FilePath { get; private set; }

        public string Directory { get; private set; }

        public void AddProjects(params string[] projects)
        {
            Projects.AddRange(projects.Where(d=>!Projects.Any(ep=>ep.Name==d)).Select(p=> new Project(p,this)));
        }

        public List<Project> Projects { get; private set; }
    }

    public class Project:IEquatable<Project>
    {
        public static string StaticName = "";
  
        /// <summary>
        /// Use it if you have only one project to build. It uses the 
        /// </summary>
        public static Project Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Project(StaticName);
                }
                return _instance;
            }

        }

        public Project(string name, Solution solution = null)
        {
            name.MustNotBeEmpty();
            _name = name;
            if (solution == null)
            {
                solution = Solution.Instance;    
                solution.Projects.AddIfNotPresent(this);
            }
            _solution = solution;
            AssemblyExtension = "dll";
        }

        /// <summary>
        /// Default is "dll"
        /// </summary>
        public string AssemblyExtension { get; set; }


       
        /// <summary>
        /// Returns csproj file path
        /// </summary>
        /// <value></value>
        public string AsProjectFile
        {
            get { return Path.Combine(Solution.Directory, Name, Name + ".csproj"); }
        }


        private string _name;
        private readonly Solution _solution;
   
        public string ReleaseDir
        {
            get { return Path.Combine(Solution.Directory, Name, "bin", "Release"); }
        }

        /// <summary>
        /// Gets path including assembly name and extension
        /// </summary>
        public string AssemblyReleasePath
        {
            get { return Path.Combine(ReleaseDir, Name + "." + AssemblyExtension); }
        }

        public string Name
        {
            get { return _name; }
        }

        public Solution Solution
        {
            get { return _solution; }
        }

        /// <summary>
        /// Returns assembly version with added pre reslease and build strings if specified
        /// </summary>
        /// <param name="preReleaseSuffix"></param>
        /// <param name="buildSuffix"></param>
        /// <returns></returns>
        public string GetAssemblySemanticVersion(string preReleaseSuffix = null, string buildSuffix = null)
        {
            return AssemblyReleasePath.GetAssemblyVersion().ToSemanticVersion(preReleaseSuffix, buildSuffix).ToString();
        }

        public string PackageDir = @"temp/package";
        private static Project _instance;
        public bool Equals(Project other)
        {
            return other != null && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Project);
        }
    }
}