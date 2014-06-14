using FluentAssertions;
using MakeSharp;
using Tests.Fixture;
using Xunit;

namespace Tests
{
    public class TaskConfigurationAttributeDependeciesTests
    {
        
        [Fact]
        public void Clean_has_no_dependencies()
        {
            var sut = new TaskConfiguration(typeof(MyWrapperClass.Clean));
            sut.GetDependencies(Setup.FakeScriptParamsObject()).Should().BeEmpty();
        }

        [Fact]
        public void Dummy_depends_on_Clean()
        {
            var sut = new TaskConfiguration(typeof(MyWrapperClass.Dummy));
            var deps=sut.GetDependencies(Setup.FakeScriptParamsObject());
            deps[0].Should().Be<MyWrapperClass.Clean>();
        }

        [Fact]
        public void Pack_depends_on_Build_and_Dummy()
        {
            var sut = new TaskConfiguration(typeof(MyWrapperClass.Pack));
            var deps = sut.GetDependencies(Setup.FakeScriptParamsObject());
            deps[0].Should().Be<MyWrapperClass.Build>();
            deps[1].Should().Be<MyWrapperClass.Dummy>();
        }
    }
}