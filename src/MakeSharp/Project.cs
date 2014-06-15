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