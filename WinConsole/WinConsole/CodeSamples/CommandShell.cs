using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Atomic.Core;
using System.Text.RegularExpressions;

namespace WinConsole.CodeSamples
{
    public class CommandShell : AtomicProcess
    {
        public CommandShell()
        {
            List<IValue> valueList = new List<IValue>();
            IValue val = Undefined.Value;

            val = new AtomicValue() { Name = "Current Command Text", Value = "" };
            valueList.Add(val);
            Values = valueList.ToArray();

            List<ITask> taskList = new List<ITask>();
            ITask task = Undefined.Task;

            task = new AtomicTask() 
            { 
                Name = "Get Command", 
                StartCondition = new RuleCondition() 
                {
                    Conditions = new ICondition[] 
                    {
                        AtomicProcess.TaskStateCondition(this, RunState.Running),
                        new ValueCondition() {
                            Value = Values[0],
                            ExpectedValue = new AtomicValue() { Value = "" }
                        }
                    }, 
                    MetFunction = RuleCondition.AllConditionsMet
                },
                Values = new IValue[] 
                {
                    new ListItemView() 
                    {
                        Name = "commandName",
                        SourceValue = new SplitStringView() 
                        { 
                            SourceValue = new TextView() { SourceValue = Values[0] }, 
                            Expression = " "
                        }, 
                        Index = new IndexView() 
                        { 
                            SourceValue = new AtomicValue() { Value = 1 } 
                        }
                    }
                }
            };
            taskList.Add(task);

            task = new AtomicTask()
            {
                Name = "Open Event Task",
                FunctionText = "open file",
                Values = new IValue[] 
                {
                    Values[0]
                },
                StartCondition = new ValueCondition()
                {
                    Value = taskList[1].GetValue("commandName"),
                    ExpectedValue = new AtomicValue() { Value = "open" }
                }
            };
            taskList.Add(task);

            task = new AtomicTask()
            {
                Name = "Close Event Task",
                FunctionText = "close file",
                Values = new IValue[] 
                {
                    Values[0]
                },
                StartCondition = new ValueCondition()
                {
                    Value = taskList[1].GetValue("commandName"),
                    ExpectedValue = new AtomicValue() { Value = "close" }
                }
            };
            taskList.Add(task);

            Tasks = taskList.ToArray();
    
            DoneCondition = new ValueCondition()
            {
                Name = "Is Quit Command",
                Value = taskList[1].GetValue("commandName"),
                ExpectedValue = new AtomicValue() { Value = "quit" }
            };
        }
    }

    class SplitStringView : ListView
    {
        public new TextView SourceValue { get; set; }

        public string Expression { get; set; }

        public override object[] Value
        {
            get
            {
                List<object> valueList = new List<object>();
                Regex reg = new Regex(Expression);
                MatchCollection matchCol = reg.Matches(SourceValue.Value);

                foreach (Match match in matchCol)
                {
                    valueList.Add(match.Value);
                }

                return valueList.ToArray();
            }
        }
    }
}
