using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Atomic.Core;
using Atomic.Loader;
using Atomic.Loader.Xml;

using WinConsole.Containers;

namespace WinConsole
{
    /*
    public class ExportContainer : BaseContainer
    {
        public DirectoryInfo DirectoryPath { get; set; }

        public IDataConverter Converter { get; set; }

        public override void Run()
        {
            foreach (IProcess p in ProcessList)
            {
                IProcessModel model = Converter.Model;
                model.Import(p);

                //  create JSON text
                string exportText = Converter.Export();

                // write JSON text to file
                FileStream fs = null;
                StreamWriter writer = null;

                fs = new FileStream(DirectoryPath.FullName + "/" + p.Name + "." + Converter.FileExtension, FileMode.Create);
                writer = new StreamWriter(fs);
                writer.Write(exportText);
                writer.Flush();
                writer.Close();
            }
        }

        public override string ExecuteFunction(string functionText, IValue resultValue)
        {
            return "";
        }

        public override void HandleError(string errorText)
        {
            throw new NotImplementedException();
        }
    }
    */


    class Program
    {
        private static IList<IContainer> _containers = new List<IContainer>();
        static void Main(string[] args)
        {
            // define the containers
            _containers.Add(new ConsoleContainer());
            _containers.Add(new GeneralContainer());

            // create the process list
            List<IProcess> processList = new List<IProcess>();
            IProcess testProcess = new WinConsole.CodeSamples.CommandShellStep03();
            processList.Add(testProcess);
            
            // add the debug container
            UniversalContainer debugContainer = new DebugInfoContainer();
            debugContainer.AddWatch(testProcess);

            foreach (ITask task in testProcess.Tasks)
            {
                debugContainer.AddWatch(task);
            }

            _containers.Add(debugContainer);

            /*           
             * IContainer container = new ExportContainer()
                        {
                            DirectoryPath = new DirectoryInfo("../../Samples"),
                            Converter = new PlainTextConverter()
                        };

                        foreach (ITask task in p.Tasks)
                        {
                            container.AddTask(task);
                        }

                        while (container.TaskList.Length > 0)
                        {
                            container.Run();
                        }
                        */
            /*
            foreach (ITask task in testProcess.Tasks)
            {
                IContainer c = testProcess.GetContainer(task.GetType());
                c.AddTask(task);
            }
            */

            // initialize the loop variables
            int step = 0;

            // repeat until no processes are running
            while (processList.Count() > 0)
            {
                DoStep(processList, ++step);
            }


            /*
            // open model file
            DirectoryInfo samplesDir = new DirectoryInfo("../../Samples");
            FileInfo xmlFile = new FileInfo(samplesDir.FullName + "/base.xml");

            StreamReader reader = new StreamReader(new FileStream(xmlFile.FullName, FileMode.Open));
            string xmlText = reader.ReadToEnd();
            reader.Close();

            // create process
            XmlConverter xmlConvert = new XmlConverter();
            xmlConvert.Import(xmlText);
            IProcess p = xmlConvert.Model.Export();

            // do a test run
            RunCoreProgram(p);

            Console.WriteLine("\nPress any key to continue.");
            Console.ReadKey();
            */
            /*
            // export JSON model
            JsonConverter jsonConvert = new JsonConverter();
            ExportToFile(jsonConvert, p, ".json");

            // export XML model
            XmlConverter xmlConvert = new XmlConverter();
            ExportToFile(xmlConvert, p, ".xml");

            // export BPMN xml
            XslTransformConverter bpmnConvert = new XslTransformConverter("../../XSL/AppXML-To-BPMN.xslt");
            ExportToFile(bpmnConvert, p, ".bpmn20.xml");

            // export plain text 
            PlainTextConverter textConvert = new PlainTextConverter();
            ExportToFile(textConvert, p, ".txt");
            */

            /*
            IProcess p = new Atomic.Samples.Countdown() { Name = "Countdown" };
            Atomic.Loader.ElementRegistry reg = new Atomic.Loader.ElementRegistry();

            reg.Import(p);

            XmlEntryConverter xmlConverter = new XmlEntryConverter();
            JsonEntryConverter jsonConverter = new JsonEntryConverter();

            ExportRegistry(reg);

            Atomic.Loader.ElementRegistry newReg = ImportRegistry("Countdown");
            IProcess serializedProcess = (IProcess)RegistryEntry.ToElement(newReg.Process);

            serializedProcess.StartEvent.Values[0].Value = Console.Out;

            RunCoreProgram(serializedProcess);
            */

            Console.WriteLine("\nPress any key to continue.");
            Console.ReadKey();
        }

        static public void DoStep(List<IProcess> processList, int step)
        {
            List<IProcess> activeList = new List<IProcess>();
            Console.WriteLine("Current Step: {0}", (step));

            // clear modified flag from each process
            foreach (IProcess p in processList)
            {
                if (p.Locked)
                {
                    p.ResetValues();
                }
                else
                {
                    p.Locked = true;

                    // add the process tasks to the containers in the set
                    foreach (IContainer container in _containers)
                    {
                        container.Add(p);
                    }
                }

                Console.WriteLine("----------------------------------------------");
            }

            // run the tasks assigned to the container
            foreach (IContainer c in _containers)
            {
                c.Run();
            }

            // update each process, setting state and output values
            foreach (IProcess p in processList)
            {
                p.Update();
            }

            // reset the active process list
            activeList.Clear();

            // remove tasks from containers from done processes
            foreach (IProcess p in processList)
            {
                if (p.CurrentState == RunState.Done)
                {
                    foreach (IContainer c in _containers)
                    {
                        c.Remove(p);
                    }
                }
                else
                {
                    activeList.Add(p);
                }
            }

            // reset the process list
            processList.Clear();
            processList.AddRange(activeList);
        }

        /*
        private static void ExportRegistry(Atomic.Loader.ElementRegistry reg)
        {
            IDictionary<string, object> exportData = reg.Export();

            // Export as JSON text
            JsonEntryConverter jsonConverter = new JsonEntryConverter();
            string appName = reg.Process.Attribute("name");
            string samplesDir = "../../../Atomic.Samples/";
            DirectoryInfo dir = new DirectoryInfo(samplesDir);

            string filename = dir.FullName + appName + "/" + appName + ".txt";

            StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8)
            {
                AutoFlush = true
            };

            writer.Write(jsonConverter.Export(exportData));
            writer.Close();

            // Export as XML text
            XmlEntryConverter xmlConverter = new XmlEntryConverter();
            filename = dir.FullName + appName + "/" + appName + ".xml";

            writer = new StreamWriter(filename, false, Encoding.UTF8)
            {
                AutoFlush = true
            };

            writer.Write(xmlConverter.Export(exportData));
            writer.Close();
        }

        static Atomic.Loader.ElementRegistry ImportRegistry(string appName)
        {
            Atomic.Loader.ElementRegistry reg = new Atomic.Loader.ElementRegistry();
            string samplesDir = "../../../Atomic.Samples/";
            DirectoryInfo dir = new DirectoryInfo(samplesDir);

            string filename = dir.FullName + appName + "/" + appName + ".txt";
            StreamReader reader = new StreamReader(filename);

            // read content from file
            string content = reader.ReadToEnd();
            reader.Close();

            // get import structure
            JsonEntryConverter jsonConverter = new JsonEntryConverter();
            IDictionary<string, object> importData = jsonConverter.Import(content);

            reg.Import(importData);
            reg.Process.Registry = reg;

            return reg;
        }
        */

        static void RunCoreProgram(Atomic.Core.IProcess p)
        {
            bool done = false;
            int step = 0;
            p.Locked = true;

            while (!done)
            {
                Console.WriteLine("Current step: {0}...", (step++));
                p.Run();
                p.Update();

                done = (p.CurrentState == Atomic.Core.RunState.Done);
            }
        }
        // * */

        static private void ExportToFile(IDataConverter convert, IProcess p, string fileExt)
        {
            // create JSON model
            IProcessModel model = convert.Model;
            model.Import(p);

            //  create JSON text
            string exportText = convert.Export();

            // write JSON text to file
            FileStream fs = null;
            StreamWriter writer = null;

            fs = new FileStream("../../Samples/" + p.Name + fileExt, FileMode.Create);
            writer = new StreamWriter(fs);
            writer.Write(exportText);
            writer.Flush();
            writer.Close();
        }
    }

    class DebugInfoContainer : UniversalContainer
    {
        static public void DisplayProcessInfo(IRunnable process)
        {
            IProcess p = (IProcess)process;

            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("---- DEBUG INFO FOR PROCESS {0} ", p.Name);
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("State: {0}\t\tDone? {1}", p.CurrentState, p.DoneCondition.Met);
            Console.WriteLine("Start? {0}\t\tStop? {1}", p.StartEvent.Met, p.StopEvent.Met);
            Console.WriteLine("Modified? {0}\t\t", p.Modified);

            foreach (IValue v in p.Inputs)
            {
                DisplayValueInfo("Input", v);
            }

            foreach (IValue v in p.Outputs)
            {
                DisplayValueInfo("Output", v);
            }

            Console.WriteLine("----------------------------------------------");
        }

        private static void DisplayValueInfo(string valueType, IValue value)
        {
            Console.WriteLine("{0} <{1}> = \"{2}\"{3}", valueType, value.Name, value.Value, (value.Modified) ? "*" : "");
        }

        static public void DisplayTaskInfo(IRunnable task)
        {
            ITask t = (ITask)task;

            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("---- DEBUG INFO FOR TASK {0} ", t.Name);
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("State: {0}\t\t", t.CurrentState);
            Console.WriteLine("Start? {0}\t\tStop? {1}", t.StartCondition.Met, t.StopCondition.Met);
            Console.WriteLine("Modified? {0}\t\t", t.Modified);

            foreach (IValue v in t.Inputs)
            {
                DisplayValueInfo("Input", v);
            }

            foreach (IValue v in t.Outputs)
            {
                DisplayValueInfo("Output", v);
            }

            Console.WriteLine("----------------------------------------------");
        }

        public DebugInfoContainer() 
        {
            ProcessRunFunction = DisplayProcessInfo;
            TaskRunFunction = DisplayTaskInfo;
        }
    }
}
