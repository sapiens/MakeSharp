using System;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using FluentAssertions;
using MakeSharp;
using Tests.Fixture;
using Xunit;

namespace Tests
{
    public class TasksListTests
    {
        private TasksList _sut;
        private TaskContext _context;

        public TasksListTests()
        {
            _sut = new TasksList(Setup.Configuration(c =>
            {
                c.MethodToExecute(m => m.Name == "Execute");
            }));
            _context = Setup.Context();
            _context.RequestedTask = "Build";
            AddClean();
            AddBuild();
            AddDummy();
        }

        void AddBuild()
        {
            var cfg = _sut.CreateConfiguration(typeof (MyWrapperClass.Build));
            cfg.Instance = new MyWrapperClass.Build(cfg);            
        }

        void AddClean()
        {
            var cfg = _sut.CreateConfiguration(typeof(MyWrapperClass.Clean));
            var instance = new MyWrapperClass.Clean();
            cfg.Instance = instance;
            instance.Context = _context;
        }

        void AddDummy()
        {
            var cfg = _sut.CreateConfiguration(typeof(MyWrapperClass.Dummy));
            var instance = new MyWrapperClass.Dummy();
            cfg.Instance = instance;
            instance.Context = _context;
        }


        [Fact]
        public void build_executor_with_dependencies_in_correct_order()
        {
            var exec = _sut.BuildExecutor(_context);
            exec.TaskDependencies.Should().ContainInOrder(typeof (MyWrapperClass.Clean), typeof (MyWrapperClass.Dummy));
        }


        [Fact]
        public void tasks_dependencies_have_their_own_dependencies_in_correct_order()
        {
            var exec = _sut.BuildExecutor(_context);
            var dep = exec.Dependencies.First(d => d.Type == typeof (MyWrapperClass.Dummy));
            dep.TaskDependencies.Should().BeEquivalentTo(typeof (MyWrapperClass.Clean));
        }
    }
}