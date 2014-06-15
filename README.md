MakeSharp (make#)
=================
Use C# scripts to automate the building process. Powered by [script cs](https://github.com/scriptcs/scriptcs). 

License
========
Apache 2.0

##Usage

In Make# any defined type can be a task. I took the decision to use clases instead of functions for encapsulation and maintainability reasons i.e to fully use the OOP mindset.
While a lot of build scripts are simple enough (a few lines of code) for other you need a bit more structure. And if you're already a C# dev, you can use your C# and OOP knowledge to code maintainable build scripts.
You can put all tasks in one file or each in its own file and then reference those in the main script. I'd say that if you're dealing with simple scripts use [CSake](https://github.com/sapiens/csake) for advanced scripts use Make# .

The default convention is that any public class is a task. Each task has an entry point, a method with no arguments named "Run". You can use your custom convention like this.

```csharp

MakeSharp.Configure
            .TasksAre(type =>  /* match */)
            .MethodToExecute(method =>  /* match */);

```

In its simple form, a task looks like this

```csharp

[Default] //executed if no task is specified
[Depends("Task1","Task2")]//straight deps on other tasks
public class SomeTask
{
    public void Run()
    {
        //do stuff
    }
}

```

For advanced scenarios you have this

```csharp

public class Build
{
    public Build(IConfigureTask cfg)
    {
        cfg.When(init=>init.ScriptParams[0]=="mono").DependOn<PrepareMono>();
        cfg.When(init=>init.ScriptParams[0]=="no-build").DontExecute();
    }
    
    public ITaskContext Context {get;set;}
    
        public void Run()
    {
        if (Context.InitData.ScriptParams[0]=="mono")
        {
        DoMono();        
       }
       
       //do some task
       
       Context.Data["some_value"]="some result";
    }
    
    void DoMono()
    {
    
    }
}

[Depends("Build")]
public class AfterBuild
{
    public ITaskContext Context {get;set;}    
    public void Run()
    {
        var result=Context.Data["some_value"].As<string>();
        //do something with result
    }
}

```

Injecting _IConfigureTask_ into the task constructor allows you to decide when a task should execute and what are the dependecies for given script arguments. Defining the _ITaskContext_ property, allows you to use the context object which gives you access to script arguments and a data dictionary you can use to pass data to other tasks.

You can have use very own implementation of _IScriptParams_ . Jsut define it in the script and Make# will use it automatically.

```csharp

class MyInitObject:IScriptParams
{
    public MyInitObject()
        {
            ScriptParams=new Dictionary<int, string>();
        }
        public IDictionary<int, string> ScriptParams { get; set; }
        
       //other properties 
}


```

### Helpers

Make# comes with 3 useful helpers classes as well as [extension methods](https://github.com/sapiens/csake/wiki/Helpers). I strongly suggest to create the scripts using an editor with intellisense. Personally I add a new project, reference MakeSharp.exe and MakeSharp.Windows.Helpers.dll then write the tasks as normal code in Visual Studio. 

* **Solution** 
 * For one solution just set the filename then use the singleton
   ```
      Solution.FileName="..\src\myProject.sln";
      //then use it 
      Solution.Instance.FilePath
   ```
* **Project** , contains properties and methods to get the release assembly path,  project file path, the semantic version of assembly.
 * If you have only one project you can use the singleton (note that you still need to define the solution file)
```
    Project.StaticName="myproject";
    Project.Current.ReleaseDir
```
* **BuildScript** gives you paths about script directory and hepls you manage temporary build folders for projects (useful when creating nuget packs, copying things etc).

Here's how Make# nuget pack generation is implemented (using predefined helpers)

```csharp

[Depends("build")]
public class pack
{
   	
	public void Run()	
    {
       var nuspec = BuildScript.GetNuspecFile(Project.Current.Name);
        nuspec.Metadata.Version = Project.Current.GetAssemblySemanticVersion();
        var tempDir = BuildScript.GetProjectTempDirectory(Project.Current);
        nuspec.Save(tempDir).CreateNuget(Solution.Instance.Directory,tempDir);        
    }
}
```
todo a more complex example involving several projects

