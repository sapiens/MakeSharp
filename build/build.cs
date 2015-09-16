using System;
using System.Collections.Generic;
using System.Reflection;
using CavemanTools;
using MakeSharp;
using MakeSharp.MsBuild;
using NuGet;
using CavemanTools.Logging;
//class h
//{
//    void b()
//    {
        Project.StaticName = "MakeSharp";
        
        Solution.FileName = @"..\src\"+Project.StaticName+".sln";  
        //Solution.FileName = @"..\src\MakeSharp.sln";  
        //Solution.Instance.AddProjects("MakeSharp","MakeSharp.Windows.Helpers");


       
    
        
		Project.Current.AssemblyExtension = "dll";
       LogManager.OutputToConsole();

//    }
//}



public class clean
 {
     public void Run()
     {

	  BuildScript.TempDirectory.CleanupDir();
      
      Solution.Instance.FilePath.MsBuildClean();
    }

   
}

public class UpdateVersion
{
    public ITaskContext Context { get; set; }
    public void Run()
    {
        var ver = Context.InitData.Get<string>("v");
		if (ver == null)
        {
			//bump=minor|patch
			var bump=Context.InitData.Get<string>("bump");			
			ver=GetVersion(bump);
			if (bump==null) return;
        }
        var info = Project.Current.GetAssemblyInfo();
       
       
        info.Info.Version = info.Info.FileVersion = ver;
        info.Save();
      
        ("Version updated to "+ver).ToConsole();
        Context.Data["version"] = ver;
    }

    string GetVersion(string bump=null)
    {
        var info=Project.Current.GetAssemblyInfo();
		if (bump=="minor") info.Info.BumpMinorVersion();
		if (bump=="patch") info.Info.BumpPatchVersion();
		Context.Data["version"] = info.Info.Version;
        ("Using version "+info.Info.Version).ToConsole();
		return info.Info.Version;
    }
}

[Default]
[Depends("clean","UpdateVersion")]
public class build
{
    public ITaskContext Context { get; set; }

    public void Run()
    {
        Solution.Instance.FilePath.MsBuildRelease();
    }
}


[Depends("build")]
public class pack
{
    public ITaskContext Context { get; set; }


    public void Run()
    {
        "template.nuspec".CreateNuget(s =>
        {
            s.Metadata.Version = Context.Data["version"].ToString();
            if (Context.InitData.HasArgument("pre"))
            {
                s.Metadata.Version += "-pre";
            }
            foreach (var dep in Project.Current.DepsList)
            {
                var ver = Project.Current.ReleasePathForAssembly(dep+".dll").GetAssemblyVersion();
                s.AddDependency(dep, ver.ToString(3));
            }
                      
        }, p =>
        {
            p.OutputDir = BuildScript.TempDirName;
            p.BasePath = Project.Current.Directory;
          
            p.BuildSymbols = false;
            p.Publish = Context.InitData.HasArgument("push");

        });
     
    }
}








