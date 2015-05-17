using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Atomic.Core;
namespace WinConsole.Containers
{
    public class DebugContainer : AtomicContainer
    {
        static private IList<DebugTaskState[]> _expectedStates = new List<DebugTaskState[]>();
 
        protected override System.Reflection.MethodInfo GenerateMethod(IFunction func)
        {
            throw new NotImplementedException();
        }

        static public void CheckStatus(IRunnable task)
        {

        }
    }

    public class DebugTaskState
    {
        public IRunnable Task { get; set; }

        public IDictionary<DebugStateProperty, object> Property { get; set; }
    }

    public enum DebugStateProperty
    {
        Modified, Met, RunState, Value
    }
}
