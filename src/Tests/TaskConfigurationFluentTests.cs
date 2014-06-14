using FluentAssertions;
using MakeSharp;
using Tests.Fixture;
using Xunit;

namespace Tests
{
    public class TaskConfigurationFluentTests
    {
        [Fact]
        public void Build_depends_on_Clean_and_Dummy()
        {
            var sut = new TaskConfiguration(typeof(MyWrapperClass.Build));
            var build = new MyWrapperClass.Build(sut);
            var deps = sut.GetDependencies(Setup.FakeScriptParamsObject());
            deps[0].Should().Be<MyWrapperClass.Clean>();
            deps[1].Should().Be<MyWrapperClass.Dummy>();
        }

        [Fact]
        public void Build_shouldnt_execute_when_condition()
        {
            var sut = new TaskConfiguration(typeof(MyWrapperClass.Build));
            var build = new MyWrapperClass.Build(sut);
            var init = Setup.FakeScriptParamsObject();
            
            sut.ShouldExecute(init).Should().BeTrue();
            init.ScriptParams[0] = "skip";
            sut.ShouldExecute(init).Should().BeFalse();
        }
    }
}