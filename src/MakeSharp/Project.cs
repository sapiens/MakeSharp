using System;
using System.Collections.Generic;
using System.IO;

namespace MakeSharp
{
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
            ReleaseDirOffset = "";
        }

        /// <summary>
        /// Default is "dll"
        /// </summary>
        public string AssemblyExtension { get; set; }

        public string ReleaseDirOffset { get; set; }
       
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
            get { return Path.Combine(Solution.Directory, Name, "bin", "Release",ReleaseDirOffset); }
        }

        public string Directory
        {
            get { return Path.Combine(Solution.Directory, Name); }
        }

        /// <summary>
        /// Gets realease path for project assembly
        /// </summary>
        public string AssemblyReleasePath
        {
            get { return ReleasePathForAssembly(); }
        }

        /// <summary>
        /// Gets path including for specified asssembly. If not specified, then the project name is used
        /// </summary>
        public string ReleasePathForAssembly(string name=null)
        {
            return Path.Combine(ReleaseDir,name??(Name + "." + AssemblyExtension)); 
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
            return ReleasePathForAssembly().GetAssemblyVersion().ToSemanticVersion(preReleaseSuffix, buildSuffix).ToString();
        }

        
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