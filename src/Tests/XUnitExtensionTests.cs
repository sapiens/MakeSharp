using MakeSharp;
using Xunit;

namespace Tests
{
    public class XUnitExtensionTests
    {
        [Fact]
        public void Run_Xunit_Extension_False()
        {
            var results = "something.dll".XUnit();
            Assert.False(results);
        }

        [Fact]
        public void Run_Xunit_Extension_True()
        {
            var results = @"..\..\Resources\Tests.dll".XUnit();
            Assert.True(results);
        }
    }
}
