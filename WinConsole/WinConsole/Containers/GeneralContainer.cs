using System;
using System.Text;
using System.Reflection;

using Atomic.Core;

namespace WinConsole.Containers
{
    public interface IGeneralContainer { }

    public class GeneralContainer : AtomicContainer, IGeneralContainer
    {
        static public void SetValue(IRunnable task)
        {
            string[] tokens = task.RunFunction.FunctionTokens;

            // find parameters and replace with id values
            IValue outputValue = task.GetOutput(tokens[1]);
            string newValue = UnQuoteText(tokens[3]);
            
            // display the final text
            outputValue.Value = newValue;
        }

        private bool IsSetCommand(string functionText)
        {
            return functionText.ToUpper().StartsWith("LET ");
        }

        protected override MethodInfo GenerateMethod(IFunction func)
        {
            TaskFunction funcMethod = null;

            if (IsSetCommand(func.FunctionText))
            {
                funcMethod = SetValue;
            }
 
            return (funcMethod == null) ? null : funcMethod.GetMethodInfo();
        }
    }
}
