using System;
using System.Linq;
using System.Reflection;
using Autofac;

namespace MakeSharp
{
    public class ScriptRunner
    {
         private readonly Assembly _asm;
        private MakeSharpConfiguration _configurator;
        public ScriptRunner(Assembly asm,MakeSharpConfiguration config=null)
        {
            _asm = asm;
            _configurator = config??Make.ConfiguratorInstance;
            FillOverrides();

            _context = _configurator.CreateContext(_initObject);
        }


        public const string InjectionCode = @"
var csRun=new ScriptRunner(System.Reflection.Assembly.GetExecutingAssembly());
csRun.Do(); 
";


        private IScriptParams _initObject;


        void FillOverrides()
        {
            var overType = _asm.GetTypes().FirstOrDefault(t => t.DerivesFrom<IScriptParams>());
            if (overType != null)
            {
                _initObject = overType.CreateInstance() as IScriptParams;
            }
            else
            {
                _initObject=new DefaultInitObject();
            }
            var i = 0;
            _configurator.ScriptArguments.ForEach(d =>
            {
                _initObject.ScriptParams[i] = d;
                i++;
            });

        }

        public void Do()
        {
           BuildContainer();

            GetDefinedTasks();

            try
            {
                var executor = _tasks.BuildExecutor(_context);
                executor.Execute(_context);  
                _context.Timer.WriteReportToConsole();
            }
            catch (InvalidOperationException ex)
            {
                ex.Message.WriteError();
            }
            

          
        }

        private void GetDefinedTasks()
        {
            _tasks = new TasksList(_configurator);
            _asm.GetTypes().Where(t=>_configurator.IsTask(t)).ForEach(t =>
            {
                var cfg=_tasks.CreateConfiguration(t);
                _currentTaskConfigurator = cfg;
                cfg.Instance = _container.Resolve(t);
            });
        }

       
        private TaskContext _context;

        private IContainer _container;
        private TasksList _tasks;

        private IConfigureTask _currentTaskConfigurator;
        

      

        void BuildContainer()
        {
            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(_asm).Where(t=>t.IsNested).PropertiesAutowired();
            cb.Register(c => _initObject).As<IScriptParams>();
            cb.Register(c => _context).As<ITaskContext>();
            cb.Register(c=>_currentTaskConfigurator).As<IConfigureTask>();
            _container = cb.Build();
        }
    }
}