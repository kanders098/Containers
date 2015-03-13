using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Atomic.Core;

namespace WinConsole.CodeSamples
{
    public class CommandShellStep01 : AtomicProcess
    {
        public CommandShellStep01()
        {
            List<ITask> taskList = new List<ITask>();
            ITask task = null;

            task = new GeneralTask()
            {
                Name = "DisplayBanner",
                FunctionText = "print \"Atomic Platform [Version 1.0]\"",
                StartCondition = AtomicProcess.TaskStateCondition(this, RunState.Running)
            };
            taskList.Add(task);

            Tasks = taskList.ToArray();

            DoneCondition = AtomicProcess.TaskStateCondition(task, RunState.Done);
        }
    }

    public class CommandShellStep02 : CommandShellStep01
    {
        public CommandShellStep02()
        {
            List<ITask> taskList = new List<ITask>();
            taskList.AddRange(Tasks);

            ITask task = null;
            task = GetTask("DisplayBanner");
            task.StartCondition = AtomicProcess.TaskStateCondition(this, RunState.Starting);

            task = new GeneralTask()
            {
                Name = "Get Command",
                FunctionText = "input \">\", cmd",
                StartCondition = AtomicProcess.TaskStateCondition(this, RunState.Running),
                Values = new IValue[] 
                {
                    new AtomicValue() { Name = "cmd", Value = "" }
                }
            };
            taskList.Add(task);

            task = new GeneralTask()
            {
                Name = "Display Unknown Command Message",
                FunctionText = "print \"Unknown command: {$cmd}.\"",
                StartCondition = AtomicProcess.TaskStateCondition(taskList[1], RunState.Done),
                Values = new IValue[] { taskList[1].GetValue("cmd") }
            };
            taskList.Add(task);

            Tasks = taskList.ToArray();

            DoneCondition = AtomicProcess.TaskStateCondition(Tasks[2], RunState.Done);
        }
    }

    public class CommandShellStep03 : CommandShellStep02
    {
        public CommandShellStep03()
        {
            List<IValue> valueList = new List<IValue>();
            valueList.AddRange(Values);

            List<ITask> taskList = new List<ITask>();
            taskList.AddRange(Tasks);

            ITask task = null;

            task = GetTask("Get Command");
            task.StartCondition = new RuleCondition()
            {
                Conditions = new ICondition[] 
                {
                    AtomicProcess.TaskStateCondition(this, RunState.Running),
                    new ValueCondition() 
                    {
                        Value = task.Values[0], 
                        ExpectedValue = new AtomicValue() { Value = "" }, 
                        MetFunction = ValueCondition.EqualsFunction
                    }
                },
                MetFunction = RuleCondition.AllConditionsMet
            };
            task.StopCondition = new ValueCondition()
            {
                Value = task.GetValue("cmd"),
                ExpectedValue = new AtomicValue() { Value = "" },
                MetFunction = ValueCondition.NotEqualsFunction
            };

            ITask unknownTask = GetTask("Display Unknown Command Message");
            unknownTask.StartCondition = new RuleCondition()
            {
                Conditions = new ICondition[] 
                { 
                    AtomicProcess.TaskStateCondition(GetTask("Get Command"), RunState.Done),
                    new ValueCondition() 
                    {
                        Value = GetTask("Get Command").GetValue("cmd"),
                        ExpectedValue = new AtomicValue() { Value = "quit" },
                        MetFunction = ValueCondition.NotEqualsFunction
                    }
                }, 
                MetFunction = RuleCondition.AllConditionsMet
            };

            task = new GeneralTask()
            {
                Name = "Clear Command",
                FunctionText = "let cmd = \"\"",
                StartCondition = AtomicProcess.TaskStateCondition(unknownTask, RunState.Done),
                Values = new IValue[] 
                { 
                    GetTask("Get Command").GetValue("cmd")
                }
            };
            taskList.Add(task);

            Tasks = taskList.ToArray();

            DoneCondition = new ValueCondition()
            {
                Value = Tasks[1].Values[0],
                ExpectedValue = new AtomicValue() { Value = "quit" }
            };
        }
    }

    public class CommandShellStep04 : CommandShellStep03
    {
        public CommandShellStep04()
        {
            Tasks[2] = new GeneralTask()
            {
                Name = "Add task to container",
                StartCondition = AtomicProcess.TaskStateCondition(GetTask("Get Command"), RunState.Done),
                Values = new IValue[] 
                { 
                    GetTask("Get Command").GetValue("cmd"),
                    new AtomicValue() { Name = "_process", Value = this }, 
                    new AtomicValue() { Name = "_task", Value = Undefined.Task }
                },
                RunFunction = AddTaskToContainerFunction
            };

            ITask task = GetTask("Clear Command");
            IValue adhocValue = Tasks[2].GetValue("_task");
            task.StartCondition = new ValueCondition()
            {
                Value = new TaskStateView() { Task = new TaskView() { SourceValue = adhocValue } },
                ExpectedValue = new AtomicValue() { Value = RunState.Done }
            };
        }

        public static void AddTaskToContainerFunction(IRunnable task)
        {
            IProcess process = (IProcess)task.GetValue("_process").Value;
            IContainer container = process.GetContainer(typeof(GeneralTask));

            ITask adhocTask = new WinConsole.Containers.AdhocTask() 
            { 
                FunctionText = (string)task.GetValue("cmd").Value 
            };
            
            container.AddTask(adhocTask);
            task.GetValue("_task").Value = adhocTask;
        }
    }

}
