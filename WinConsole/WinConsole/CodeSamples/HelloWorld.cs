using Atomic.Core;

namespace WinConsole.CodeSamples
{
    public class HelloWorld : AtomicProcess
    {
        public HelloWorld()
        {
            Name = "Hello World";
            Tasks = new ITask[] {
                new AtomicTask() 
                { 
                    Name = "Display Greeting", 
                    StartCondition = new ValueCondition()
                    {
                        Name = "Is Process Running",
                        Value = new TaskStateView() { Name = "Process Current State", Task = this }, 
                        ExpectedValue = new AtomicValue() { Name = "Task Running", Value = RunState.Running }
                    }, 
                    FunctionText = "print \"Hello World!\""  
                }
            };
            DoneCondition = new ValueCondition()
            {
                Name = "Greeting Displayed",
                Value = new TaskStateView()
                {
                    Name = "Task Current State",
                    Task = Tasks[0]
                },
                ExpectedValue = new AtomicValue() { Name = "Task Done", Value = RunState.Done }
            };
        }
    }
}
