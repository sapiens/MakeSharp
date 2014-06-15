#r "..\src\MakeSharp.Windows.Helpers\bin\Debug\MakeSharp.Windows.Helpers.dll"

using System;
using System.Collections.Generic;
using System.Reflection;
using MakeSharp;
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

	public void Run()
    {
        Solution.Instance.FilePath.MsBuildRelease();
	}
}


[Depends("build")]
public class pack
{
    public ITaskContext Context {get;set;}

    
	public void Run()	
    {
       
        var nuspec = BuildScript.GetNuspecFile(Project.Current.Name);
        nuspec.Metadata.Version = Project.Current.GetAssemblySemanticVersion();
        var tempDir = BuildScript.GetProjectTempDirectory(Project.Current);
        var nupkg=nuspec.Save(tempDir).CreateNuget(Solution.Instance.Directory,tempDir);
	    Context.Data["pack"] = nupkg;
    }
}


[Depends("pack")]
public class push
{
    public ITaskContext Context { get; set; }

    
    public void Run()
    {
        var nupkg=Context.Data.GetValue<string>("pack");
     
        BuildScript.NugetExePath.Exec("push", nupkg);
    }
}





