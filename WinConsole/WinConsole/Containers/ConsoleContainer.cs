using System;
using System.Text;
using System.Reflection;

using Atomic.Core;

namespace WinConsole.Containers
{
    public class ConsoleContainer : PresentationContainer
    {
        public ConsoleContainer()
        {
            ReadFunction = ReadTextFromConsole;
            WriteFunction = WriteTextToConsole;
        }

        /// <summary>
        /// Writes a block of text to the console window.
        /// </summary>
        /// <param name="task">The task containing the task and relevant parameters</param>
        /// <remarks>The substitution has to occur here so that the current parameter values are used.</remarks>
        static public void WriteTextToConsole(IRunnable task)
        {
            string[] tokens = null;
            string message = ParseTask(task, out tokens);

            // display the final text
            Console.WriteLine(message);
        }

        static public void ReadTextFromConsole(IRunnable task)
        {
            string[] tokens = null;
            string message = ParseTask(task, out tokens);

            Console.Write(message + " ");
            task.GetOutput(tokens[3]).Value = Console.ReadLine();
        }
    }
}
