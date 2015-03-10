using Atomic.Core;

namespace WinConsole.CodeSamples
{
    public class SimpleModel : AtomicProcess
    {
        public SimpleModel()
        {
            Name = "SimpleModel";
            DoneCondition = new ValueCondition()
            {
                Value = new TaskStateView() { Name = "Process Current State", Task = this },
                ExpectedValue = new AtomicValue() { Name = "Task is Running", Value = RunState.Running }
            };
        }
    }
}
