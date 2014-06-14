using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MakeSharp
{
    class Program
    {
        static int Main(string[] args)
        {
            WriteHeader();
            if (args.Length == 0)
            {
                "Usage: makesharp script_name [task_name] [arg0] [arg1] [...]".ToConsole();
                return 0;
            }

            var file = args[0];
            if (!File.Exists(file))
            {
                "File '{0}' couldn't be found".WriteError(file);
                return 1;
            }

          
            Configure(args);

            var host = new ScriptCsHost();
            var result = host.Execute(file);
            if (result.CompileExceptionInfo != null) Console.WriteLine(result.CompileExceptionInfo.SourceException);
            if (result.ExecuteExceptionInfo != null) Console.WriteLine(result.ExecuteExceptionInfo.SourceException);
            
           // Console.ReadKey();
            return 0;
        }

        static void WriteHeader()
        {
            var version = Assembly.GetExecutingAssembly().Version();
            "make# version {0}".ToConsole(string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build));
            "Copyright (c) 2014 Mihai Mogosanu".ToConsole();
            Console.WriteLine();
        }

        private static void Configure(string[] args)
        {
            if (args.Length==1) return;
            var cfg = MakeSharp.ConfiguratorInstance;
            cfg.ScriptName = args[1];
            cfg.ScriptArguments = args.Skip(2).ToArray();            
        }
    }
}
