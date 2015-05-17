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

            task = new PresentationTask()
            {
                Name = "DisplayBanner",
                RunFunction = new AtomicFunction() 
                { 
                    FunctionText = "write \"Atomic Platform [Version 1.0]\"" 
                },
                StartCondition = AtomicProcess.AtStateCondition(this, RunState.Running)
            };
            taskList.Add(task);

            Tasks = taskList.ToArray();

            DoneCondition = AtomicProcess.AtStateCondition(task, RunState.Done);
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
            task.StartCondition = AtomicProcess.AtStateCondition(this, RunState.Starting);

            task = new PresentationTask()
            {
                Name = "Get Command",
                RunFunction = new AtomicFunction() 
                {
                    FunctionText = "read \">\" as cmd"
                },
                Outputs = new IValue[] 
                {
                    new AtomicValue() { Name = "cmd", Value = "" }
                }
            };

            task.StartCondition = new RuleCondition() 
            {
                Conditions = new ICondition[] 
                {
                    AtomicRunnable.AtStateCondition(this, RunState.Running),
                    new ValueCondition() 
                    { 
                        Value = task.GetOutput("cmd"), 
                        ExpectedValue = new AtomicValue() { Value = "" }, 
                        MetFunction = ValueCondition.EqualsFunction 
                    }
                }, 
                MetFunction = RuleCondition.AllConditionsMet
            };

            taskList.Add(task);

            task = new PresentationTask()
            {
                Name = "Display Unknown Command Message",
                RunFunction = new AtomicFunction()
                {
                    FunctionText = "write \"Unknown command: $[cmd].\""
                },
                StartCondition = AtomicProcess.AtStateCondition(taskList[1], RunState.Done),
                Inputs = new IValueView[] 
                { 
                    new TextView() { SourceValue = taskList[1].GetOutput("cmd") }
                }
            };
            taskList.Add(task);

            Tasks = taskList.ToArray();

            DoneCondition = AtomicProcess.AtStateCondition(Tasks[2], RunState.Done);
        }
    }

    public class CommandShellStep03 : CommandShellStep02
    {
        public CommandShellStep03()
        {
            // move the command output to the process level
            Outputs = new IValue[] 
            {
                new AtomicValue() { Name = "cmd", Value = "" }
            };

            List<ITask> taskList = new List<ITask>();
            taskList.AddRange(Tasks);

            ITask task = null;

            task = GetTask("Get Command");
            task.StartCondition = new RuleCondition()
            {
                Conditions = new ICondition[] 
                {
                    AtomicProcess.AtStateCondition(this, RunState.Running),
                    new ValueCondition() 
                    {
                        Value = this.GetOutput("cmd"), 
                        ExpectedValue = new AtomicValue() { Value = "" }, 
                        MetFunction = ValueCondition.EqualsFunction
                    }
                },
                MetFunction = RuleCondition.AllConditionsMet
            };
            task.StopCondition = new ValueCondition()
            {
                Value = this.GetOutput("cmd"),
                ExpectedValue = new AtomicValue() { Value = "" },
                MetFunction = ValueCondition.NotEqualsFunction
            };
            task.Outputs = new IValue[] {
                this.GetOutput("cmd")
            };

            ITask unknownTask = GetTask("Display Unknown Command Message");
            unknownTask.StartCondition = new RuleCondition()
            {
                Conditions = new ICondition[] 
                { 
                    AtomicProcess.AtStateCondition(GetTask("Get Command"), RunState.Done),
                    new ValueCondition() 
                    {
                        Value = GetInput("cmd"),
                        ExpectedValue = new AtomicValue() { Value = "quit" },
                        MetFunction = ValueCondition.NotEqualsFunction
                    }
                }, 
                MetFunction = RuleCondition.AllConditionsMet 
            };
            unknownTask.Inputs = new IValueView[] 
            {
                new TextView() { SourceValue = this.GetOutput("cmd") }
            };

            task = new GeneralTask()
            {
                Name = "Clear Command",
                RunFunction = new AtomicFunction() 
                {
                    FunctionText = "let cmd = \"\""
                },
                StartCondition = AtomicProcess.AtStateCondition(unknownTask, RunState.Done),
                Outputs = new IValue[] 
                { 
                    this.GetOutput("cmd")
                }
            };
            taskList.Add(task);

            Tasks = taskList.ToArray();

            DoneCondition = new ValueCondition()
            {
                Value = GetOutput("cmd"),
                ExpectedValue = new AtomicValue() { Value = "quit" }
            };
        }
    }
    
    /// <summary>
    /// Read/write from file
    /// </summary>
    public class CommandShellStep04
    {
        public CommandShellStep04()
        {

        }
    }
    /*
    public class CommandShellStep04 : CommandShellStep03
    {
        public CommandShellStep04()
        {

            List<IValue> valueList = new List<IValue>();
            valueList.AddRange(Values);

            IValue v = null;
            v = new AtomicValue() { Name = "runProcess", Value = new AtomicProcess() };
            valueList.Add(v);

            Values = valueList.ToArray();
 
            Tasks[2] = new SendMessageTask()
            {
                Name = "Add task to container",
                StartCondition = AtomicProcess.TaskStateCondition(GetTask("Get Command"), RunState.Done),
                TargetProcess = new ProcessView() { SourceValue = GetValue("runProcess") }, 
                Message = new AtomicMessage() 
                { 
                    Name = "AddTask", 
                    Parameters = new IParameter[]
                    {
                        new ParameterValue() 
                        { 
                            Name = "command", 
                            Value = GetTask("Get Command").GetValue("cmd")
                        }
                    }
                }
            };

            ITask task = GetTask("Clear Command");
            IValue adhocValue = Tasks[2].GetValue("_task");
            task.StartCondition = new ValueCondition()
            {
                Value = new TaskStateView() { Task = new TaskView() { SourceValue = adhocValue } },
                ExpectedValue = new AtomicValue() { Value = RunState.Done }
            };
        }
    }

    class AdhocProcess : AtomicProcess
    {
        public AdhocProcess()
        {
            List<IEvent> eventList = new List<IEvent>();
            IEvent evt = null;

            evt = new MessageEvent()
            {
                Name = "AddTask",
                Message = Undefined.Message,
                Process = this
            };
            eventList.Add(evt);

            Events = eventList.ToArray();

            List<ITask> taskList = new List<ITask>();
            ITask task = null;

            task = new GeneralTask()
            {
                Name = "Create Task",
                StartCondition = GetEvent("AddTask").StartCondition,
                Values = new IValue[] 
                { 
                    ((MessageEvent)evt).Message.GetParameter("command"), 
                    new AtomicValue() { Name = "newTask", Value = Undefined.Task }
                },
                RunFunction = CreateAdHocTask
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

        public static void CreateAdHocTask(IRunnable task)
        {
            TextView cmdView = new TextView() { SourceValue = task.GetValue("command") };
            string cmd = cmdView.Value;
        }
    }
     */ 
}
