using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Atomic.Core;

namespace WinConsole.CodeSamples
{
    public class EchoName : AtomicProcess
    {
        public EchoName()
        {
            List<ITask> taskList = new List<ITask>();
            ITask task = null;

            task = new GeneralTask()
            {
                Name = "Get Name",
                FunctionText = "input \"Hi!  What's your name?\", name",
                StartCondition = AtomicProcess.TaskStateCondition(this, RunState.Running),
                Values = new IValue[] 
                {
                    new AtomicValue() { Name = "name", Value = "" }
                }
            };
            taskList.Add(task);

            task = new GeneralTask()
            {
                Name = "Display Greeting",
                FunctionText = "print \"It's good to meet you {$name}.\"",
                StartCondition = AtomicProcess.TaskStateCondition(taskList[0], RunState.Done),
                Values = new IValue[] 
                {
                    taskList[0].GetValue("name")
                }
            };
            taskList.Add(task);

            Tasks = taskList.ToArray();

            DoneCondition = AtomicProcess.TaskStateCondition(Tasks[1], RunState.Done);
        }
    }
}
