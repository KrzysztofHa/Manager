using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Manager.Tests.ConsolTest
{
    internal class TestConsole : ITestConsole
    {
        public ITestOutputHelper OutputWriter { get; }
        
        
        public TestConsole(ITestOutputHelper outputWriter)
        {
            OutputWriter = outputWriter;
            var stream = Console.Clear();
        }
        
        public void Clear()
        {
            throw new NotImplementedException("Udało się");
        }
    }
}
