# No longer maintained

MakeSharp (make#)
=================
Use C# scripts to automate the building process. Powered by [script cs](https://github.com/scriptcs/scriptcs). 

Latest version: **[1.3.0](https://github.com/sapiens/MakeSharp/wiki/ChangeLog)**

License
========
Apache 2.0

##Usage

In Make# any defined type can be a task. I took the decision to use clases instead of functions for encapsulation and maintainability reasons i.e to fully use the OOP mindset.
While a lot of build scripts are simple enough (a few lines of code) for other you need a bit more structure. And if you're already a C# dev, you can use your C# and OOP knowledge to code maintainable build scripts.
You can put all tasks in one file or each in its own file and then reference those in the main script. I'd say that if you're dealing with simple scripts use [CSake](https://github.com/sapiens/csake) for advanced scripts use Make# .

The default convention is that any public class is a task. Each task has an entry point, a method with no arguments named "Run". If you wnt to define public utility classes (but not tasks) the convention is to end their name with an underscore e.g Utils_ . You can override this and use your custom convention.

```csharp

Make.Configure
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
        cfg.When(init=>init.ScriptParams.HasArgument("mono")).DependOn<PrepareMono>();
        cfg.When(init=>init.ScriptParams.HasArgument("no-build")).DontExecute();
    }
    
    public ITaskContext Context {get;set;}
    
    public void Run()
    {
        if (Context.InitData.ScriptParams.HasArgument("mono"))
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
        var result=Context.Data.GetValue<string>("some_value");
        //do something with result
    }
}

```

Injecting _IConfigureTask_ into the task constructor allows you to decide when a task should execute and what are the dependecies for given script arguments. Defining the _ITaskContext_ property allows you to use the context object which gives you access to script arguments and a data dictionary you can use to pass data to other tasks. You can also inject any other defined class via constructor or by defining a public property of that type.

```csharp
public class MyUtils_ {}

public class MyTask
{

 public MyUtils_ Utils {get;set;}
 public void Run()
 {
  Utils.DoSmth();
 }

}

```


You can use your very own implementation of _IScriptParams_ . Just define it in the script and Make# will use it automatically.

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

Make# comes with 3 useful helpers classes as well as [extension methods](https://github.com/sapiens/csake/wiki/Helpers). I strongly suggest to create the scripts using an editor with intellisense. Personally I have a "Builder" project that I use to write build scripts ofr all my OSS libraries. However most of the time I just change the project name and that's it.
The Builder is required only when I have to do some project specific stuff.

* **Solution** 
For one solution just set the filename then use the singleton

```csharp
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

##Example

Here's how Make# nuget pack generation is implemented (using predefined helpers)

```csharp

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
```
todo a more complex example involving several projects

