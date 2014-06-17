using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using log4net.Util;
using NuGet;

namespace MakeSharp
{
    public static class HelpersExtensions
    {
        /// <summary>
        /// Equivalent to Console.WriteLine(mystring,args)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="args"></param>
        public static void ToConsole(this string data, params object[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(data);
            }
            else
            {
                Console.WriteLine(data, args);
            }
        }

        /// <summary>
        /// Writes to console using red colour
        /// </summary>
        /// <param name="data"></param>
        /// <param name="args"></param>
        public static void WriteError(this string data, params object[] args)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            if (args.Length == 0)
            {
                Console.WriteLine(data);
            }
            else
            {
                Console.WriteLine(data, args);
            }
            Console.ForegroundColor = old;
        }

        /// <summary>
        /// Writes to console using cyan colour
        /// </summary>
        /// <param name="data"></param>
        /// <param name="args"></param>
        public static void WriteInfo(this string data, params object[] args)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            if (args.Length == 0)
            {
                Console.WriteLine(data);
            }
            else
            {
                Console.WriteLine(data, args);
            }
            Console.ForegroundColor = old;
        }

        /// <summary>
        /// Creates a directory
        /// </summary>
        /// <param name="dirName">Directory path</param>
        public static void MkDir(this string dirName)
        {
            dirName.MustNotBeEmpty();
            Directory.CreateDirectory(dirName);
        }

        /// <summary>
        /// Deletes the specified directory
        /// </summary>
        /// <param name="dirName">Directory path</param>
        public static void DeleteDir(this string dirName)
        {
            dirName.MustNotBeEmpty();
            if (Directory.Exists(dirName))
            {
                Directory.Delete(dirName, true);
            }
        }

        /// <summary>
        /// Empties the specified directory
        /// </summary>
        /// <param name="dirName">Directory path</param>
        public static void CleanupDir(this string dirName)
        {
            dirName.MustNotBeEmpty();
            if (Directory.Exists(dirName))
            {
                Directory.Delete(dirName, true);
            }
            Directory.CreateDirectory(dirName);
        }

        public static DirectoryInfo ToDirectoryInfo(this string path)
        {
            return new DirectoryInfo(path);            
        }

        public static FileInfo ToFileInfo(this string path)
        {
            return new FileInfo(path);
        }

        /// <summary>
        /// Runs executable with specified arguments. 
        /// No new window is created and all output goes to console.
        /// </summary>
        /// <param name="file">Executable name</param>
        /// <param name="args">Arguments list</param>
        /// <returns>Process exit code</returns>
        public static int Exec(this string file, params string[] args)
        {
            using (var p = new Process())
            {
                p.StartInfo.FileName = file;
                if (args.Length > 0)
                {
                    p.StartInfo.Arguments = string.Join(" ", args);
                }
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                "Executing: {0} {1}".ToConsole(file, p.StartInfo.Arguments);
                p.ErrorDataReceived += p_ErrorDataReceived;
                int result = -1;
                try
                {
                    p.Start();
                    p.BeginErrorReadLine();
                    Console.WriteLine(p.StandardOutput.ReadToEnd());
                    p.WaitForExit();
                    result = p.ExitCode;
                }
                finally
                {
                    p.ErrorDataReceived -= p_ErrorDataReceived;
                }
                return result;
            }
        }

        private static void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!e.Data.IsNullOrEmpty())
            {
                e.Data.WriteError();
            }
        }
        /// <summary>
        /// Loads the specified file as a nuspec file (nuget manifest)
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static NuSpecFile AsNuspec(this string filename)
        {
            return new NuSpecFile(filename);
        }

        /// <summary>
        /// Treats the file as an assembly and returns its version
        /// </summary>
        /// <param name="filename">Path to assembly file</param>
        /// <returns></returns>
        public static Version GetAssemblyVersion(this string filename)
        {
            var f = new FileInfo(filename);
            "Getting version from assembly '{0}'".ToConsole(f.FullName);
            var name = AssemblyName.GetAssemblyName(f.FullName);
            return name.Version;
        }


        /// <summary>
        /// Creates a semantic version (http://semver.org/) from a Version allowing you to specify pre-release and build strings.
        /// </summary>
        /// <see cref="http://semver.org/"/>
        /// <param name="version"></param>
        /// <param name="preRelease">beta => 1.0.0-beta</param>
        /// <param name="build">001 => 1.0.0-beta+001</param>
        /// <returns></returns>
        public static CavemanTools.SemanticVersion ToSemanticVersion(this Version version, string preRelease = null,
            string build = null)
        {
            version.MustNotBeNull();
            return new CavemanTools.SemanticVersion(version, preRelease, build);
        }

        /// <summary>
        /// Creates nuget package and returns the nupkg file path
        /// </summary>
        /// <param name="file"></param>
        /// <param name="basePath"></param>
        /// <param name="outputDir"></param>
        /// <returns></returns>
        public static string CreateNuget(this string file, string basePath, string outputDir)
        {
            "Creating nuget package from '{0}'".ToConsole(file);
            if (outputDir.IsNullOrEmpty())
            {
                outputDir = Path.GetDirectoryName(file);
            }

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            string nugetFile = "";
            using (var fs = File.Open(file, FileMode.Open))
            {
                var pack = new PackageBuilder(fs, Path.GetFullPath(basePath));
                var nugetName = pack.Id + "." + pack.Version.ToString() + ".nupkg";
                nugetFile = Path.Combine(outputDir, nugetName);
                using (var ws = File.Create(nugetFile))
                {
                    pack.Save(ws);
                }

            }
            "Package '{0}' was successfuly created".WriteInfo(nugetFile);
            return nugetFile;
        }
    }


  
}
