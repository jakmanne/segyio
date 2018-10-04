using System;
using Xunit;
using dotnet;

namespace XUnitTests
{
    public class UnitTests
    {
        [Fact]
        public void Test1()
        {
            Assert.True(Open.IsEvenNumberTest(2));
        }
    }
}
