using System.IO;
using System.Threading;
using Xunit.Runners;

namespace MakeSharp
{
    public static class XunitExtensionMethods
    {
        static readonly ManualResetEvent Finished = new ManualResetEvent(false);
        private static bool _result = true;

        public static bool XUnit(this string dllName)
        {
            if (!File.Exists(dllName)) return false;

            using (var runner = AssemblyRunner.WithAppDomain(dllName))
            {
                runner.OnTestSkipped += info => { _result = false; };
                runner.OnTestFailed += info => { _result = false; };
                runner.OnExecutionComplete += info => { Finished?.Set(); };
                runner.Start();

                Finished.WaitOne();
                Finished.Dispose();
            }
            return _result;
        }
    }
}