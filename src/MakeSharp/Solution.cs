using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MakeSharp
{
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
}