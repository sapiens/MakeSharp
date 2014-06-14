
using MakeSharp;
using Tests.Fixture;
using Xunit;

namespace Tests
{
    public class TaskExecutorTests
    {
        [Fact]
        public void run_task_without_dependencies()
        {
            var context = Setup.Context();
            var task = new MyWrapperClass.Clean();
            task.Context = context;
           var method = task.GetType().GetMethod("Execute");

            var exec = new TaskExecutor(task, method);
            
            exec.Execute(context);
            Assert.Equal(context.Data["Result"],23);          
        }

        //[Fact]
        //public void run_task_after_dependencies()
        //{
            
        //}
    }
}