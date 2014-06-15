#r "..\src\MakeSharp.Windows.Helpers\bin\Debug\MakeSharp.Windows.Helpers.dll"

// using System;
// using System.IO;//
// using System.Linq;
// using MakeSharp;//
using MakeSharp.Windows.Helpers;


// public class _
// {
    // public _()
    // {
        Project.StaticName = "MakeSharp";
        Solution.FileName = @"..\src\MakeSharp.sln";  
		Project.Current.AssemblyExtension = "exe";
    // }
// }



 public class clean
 {
     public void Run()
	 {

         BuildScript.TempDirectory.CleanupDir();
        Solution.Instance.FilePath.MsBuildClean();         
	 }

 }

[Default]
[Depends("clean")]
public class build
{
    public ITaskContext Context {get;set;}
	
	public void Run()
    {
        Solution.Instance.FilePath.MsBuildRelease();
		Context.Data["test"]="some value";
    }
}


[Depends("build")]
public class pack
{
    public ITaskContext Context {get;set;}
	
	public void Run()	
    {
        Context.Data["test"].As<string>().ToConsole();
		var nuspec = BuildScript.GetNuspecFile(Project.Current.Name);
        nuspec.Metadata.Version = Project.Current.GetAssemblySemanticVersion();
        var tempDir = BuildScript.GetProjectTempDirectory(Project.Current);
        nuspec.Save(tempDir).CreateNuget(Solution.Instance.Directory,tempDir);        
    }
}





