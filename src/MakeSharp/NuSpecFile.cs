using System;
using System.Collections.Generic;
using System.IO;
using NuGet;

namespace MakeSharp
{
    /// <summary>
    /// Wraps a nuspec file (nuget Manifest)
    ///  </summary>
    /// <see cref="http://docs.nuget.org/docs/reference/nuspec-reference"/>
    public class NuSpecFile
    {
        private readonly string _filename;
        private Manifest _manifest;
        private FileInfo _info;

        public NuSpecFile(string filename)
        {
            _filename = filename;
            _info = new FileInfo(filename);
            using (var fs = File.Open(filename, FileMode.Open))
            {
                _manifest = Manifest.ReadFrom(fs, false);
            }
            if (Manifest.Files == null)
            {
                Manifest.Files = new List<ManifestFile>();
            }
            if (_manifest.Metadata.DependencySets == null)
            {
                _manifest.Metadata.DependencySets = new List<ManifestDependencySet>();
            }

        }

        /// <summary>
        /// Adds a dependency of the package
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        /// <param name="framework"></param>
        public void AddDependency(string id, string version = null, string framework = null)
        {
            var set = GetDependencySet(framework);
            set.Dependencies.Add(new ManifestDependency()
            {
                Id = id,
                Version = version
            });
        }

        ManifestDependencySet GetDependencySet(string framework)
        {
            var set = Metadata.DependencySets.Find(d => d.TargetFramework == framework);
            if (set == null)
            {
                set = new ManifestDependencySet() { TargetFramework = framework };
                set.Dependencies = new List<ManifestDependency>();
                Metadata.DependencySets.Add(set);
            }
            return set;
        }

        /// <summary>
        /// Gets the manifest metadata
        /// </summary>        
        public ManifestMetadata Metadata
        {
            get
            {
                return _manifest.Metadata;
            }
        }

        /// <summary>
        /// Gets the underlying nuget manifest
        /// </summary>
        public Manifest Manifest
        {
            get
            {
                return _manifest;
            }
        }

        /// <summary>
        /// Specifies a file to be included in the package
        /// </summary>
        /// <param name="source">The location of the file or files to include. The path is relative to the NuSpec file unless an absolute path is specified. The wildcard character, *, is allowed. Using a double wildcard, **, implies a recursive directory search.</param>
        /// <param name="target">This is a relative path to the directory within the package where the source files will be placed.</param>
        /// <param name="exclude">The file or files to exclude. This is usually combined with a wildcard value in the `src` attribute. The `exclude` attribute can contain a semi-colon delimited list of files or a file pattern. Using a double wildcard, **, implies a recursive exclude pattern.</param>
        public void AddFile(string source, string target, string exclude = "")
        {
            var mf = new ManifestFile() { Exclude = exclude, Source = source, Target = target };
            _manifest.Files.Add(mf);
        }

        /// <summary>
        /// Saves the changes into nuspec file and returns the full output path
        /// </summary>
        /// <param name="outputDir">If name is null or empty it assumes it's the original directory</param>
        public string Save(string outputDir = "")
        {
            if (outputDir.IsNullOrEmpty())
            {
                outputDir = _info.DirectoryName;
            }
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
            var output = Path.Combine(outputDir, _info.Name);
            using (var fs = File.Open(output, FileMode.OpenOrCreate))
            {
                _manifest.Save(fs);
            }
            return output;
        }

    }
}