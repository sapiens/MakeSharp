using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.InteropServices;
using CavemanTools;
using Common.Logging.Simple;
using NuGet;
using ScriptCs;
using ScriptCs.Contracts;
using ScriptCs.Engine.Roslyn;

namespace MakeSharp
{
    public class ScriptCsHost
    {
        private IScriptExecutor _exec;

        public ScriptCsHost()
        {
            var logger = new ConsoleOutLogger("default", Common.Logging.LogLevel.Info, true, true, false, "dd MMM yyy hh:mm:ss");

            var scriptCsBuilder = new ScriptServicesBuilder(new ScriptConsole(), logger)
                .LogLevel(LogLevel.Debug)
                .Cache(true)
                .Repl(false)
                .ScriptEngine<RoslynScriptEngine>();

            _exec = scriptCsBuilder.Build().Executor;
        }

        
        public ScriptResult Execute(string script)
        {
            _exec.AddReferences(typeof(Percentage));
            _exec.AddReferenceAndImportNamespaces(new []{typeof(PackageBuilder),GetType(),typeof(RequiredAttribute)});
            _exec.ImportNamespaces( "System.Linq", "CavemanTools");

            _exec.Initialize(new string[0], new IScriptPack[0]);            
            return _exec.ExecuteScript(GetScriptContent(script));
        }

        string GetScriptContent(string script)
        {
            var file = File.ReadAllText(script);
            file += ScriptRunner.InjectionCode;
            return file;
        }
    }
}