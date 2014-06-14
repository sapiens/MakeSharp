using System.Text;
using FluentAssertions;
using MakeSharp;
using Tests.Fixture;
using Xunit;

namespace Tests
{
    public class ScriptRunnerTests
    {
        private ScriptRunner _sut;
        private MakeSharpConfiguration _config;

        public ScriptRunnerTests()
        {
            _config = Setup.Configuration();
            _config.TasksAre(t => t.DeclaringType == typeof (IntegrationTasks));
            _sut = new ScriptRunner(GetType().Assembly,_config);
        }

        [Fact]
        public void execute_default_task()
        {
            var sb = new StringBuilder();
            _config.Context.Data["Result"] = sb;
            _sut.Do();
            sb.ToString().Should().Be("CleanBuild");
        }
        
        [Fact]
        public void execute_pack_task()
        {
            var sb = new StringBuilder();
            _config.Context.RequestedTask = "Pack";
            _config.Context.Data["Result"] = sb;
            _sut.Do();
            sb.ToString().Should().Be("CleanBuildPack");
            _config.Context.Timer.WriteReportToConsole();
        }



    }
}