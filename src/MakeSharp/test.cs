//#r "MakeSharp.Windows.Helpers.dll"

public class Clean
        {
            public void Run()
            {
                "vvbbb".WriteInfo();
				var b=new[]{"bla"};
				b.ForEach(d=>d.ToConsole());
				"Cleaning".ToConsole();
            }

            public ITaskContext Context { get; set; }
        }
        
        [Depends("Clean")]
        [Default]
        public class Build
        {
            public void Run()
            {
                
				"Building".ToConsole();
				Context.Data["Version"]="1.0.0";
            }

            public ITaskContext Context { get; set; }
        }

        public class Pack
        {
            public Pack(IConfigureTask cfg)
            {
                cfg.Always.DependOn<Build>();
            }
            public void Run()
            {
				
				"Packing version {0}".ToConsole(Context.Data["Version"]);
            }

            public ITaskContext Context { get; set; }
        }