using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Fixture;

namespace Tests
{
    public abstract class BaseTests
    {
        public BaseTests()
        {
            MakeSharp.MakeSharp.Configure.TasksAre(t => t.IsNested && t.DeclaringType == typeof (MyWrapperClass));
        }
    }
}
