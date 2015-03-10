using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Atomic.Core;
using Atomic.Loader;
using Atomic.Loader.Xml;

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

    public class ConsoleContainer : RunContainer, IDataAccessContainer
    {
        public override string ExecuteTask(ITask task)
        {
            IDictionary<string, object> response = new Dictionary<string, object>();
            string functionText = task.FunctionText.Trim();

            string cmd = "";
            string text = "";
            string valueName = "";
            ParseFunctionText(functionText, out cmd, out text, out valueName);
            text = ReplaceValues(task, text);
            AtomicMessage message = new AtomicMessage();

            switch (cmd)
            {
                case "input":
                    message.Parameters = new IParameter[] 
                    {
                        new ParameterValue() { Name = "promptText", Value = text },
                        new ParameterValue() { Name = "inputText", InputParameter = false }
                    };

                    Read(message);
                    task.GetValue(valueName).Value = message.GetParameter("inputText").Value;
                    break;
                case "print":
                    message.Parameters = new IParameter[] 
                    {
                        new ParameterValue() { Name = "outputText", Value = text }
                    };

                    Write(message);
                    break;
                default:
                    response["error"] = "unknown command";
                    response["function"] = functionText;
                    break;
            }

            return FormatResponse(response);
        }

        private void ParseFunctionText(string functionText, out string cmd, out string text, out string valueName)
        {
            Regex ex = new Regex("(\\\")+");
            string[] tokens = ex.Split(functionText);
            cmd = tokens[0].ToLower().Trim();
            valueName = "";
            int lastToken = tokens.Length - 1;

            if (cmd == "input")
            {
                if (tokens[lastToken].StartsWith(","))
                {
                    valueName = tokens[lastToken].Substring(1).Trim();
                }
            }

            text = String.Join(" ", tokens, 2, lastToken - 3);
        }

        private string ReplaceValues(ITask task, string text)
        {
            MatchCollection matchCol = new Regex(@"{[$|\w|\s]+}").Matches(text);
            StringBuilder buffer = new StringBuilder(text);
            string paramName = "";
            IValue paramValue = Undefined.Value;

            foreach (Match valueMatch in matchCol)
            {
                paramName = valueMatch.Value;
                if (paramName[1] != '$') continue;
                paramName = paramName.Substring(2, paramName.Length - 3);
                paramValue = task.GetValue(paramName);

                if (paramValue != Undefined.Value)
                {
                    buffer.Replace(valueMatch.Value, paramValue.Value.ToString());
                }
            }

            return buffer.ToString();
        }

        public override void HandleError(string errorText)
        {
            if (errorText.Length == 0) return;
            Console.WriteLine(errorText);
        }

        public void Read(IMessage message)
        {
            string prompt = message.GetParameter("promptText").Value.ToString();
            Console.Write(prompt + " ");

            IParameter outParam = message.GetParameter("inputText");
            outParam.Value = Console.ReadLine();
        }

        public void Write(IMessage message)
        {
            Console.WriteLine(message.GetParameter("outputText").Value);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IProcess testProcess = new WinConsole.CodeSamples.EchoName();
/*            IContainer container = new ExportContainer()
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
            List<IProcess> processList = new List<IProcess>();
            processList.Add(testProcess);

            List<IContainer> containerList = new List<IContainer>();
            containerList.Add(new ConsoleContainer() 
            {
                DebugMode = false,
                DebugStream = Console.OpenStandardOutput()
            });

            foreach (ITask task in testProcess.Tasks)
            {
                containerList[0].AddTask(task);
            }

            List<IProcess> activeList = new List<IProcess>();

            while (processList.Count() > 0)
            {
                foreach (IProcess p in processList) 
                {
                    if (p.CurrentState != RunState.Running)
                    {
                        p.Update();
                        continue;
                    }

                    foreach (IContainer c in containerList)
                    {
                        c.Run();
                    }

                    if (p.DoneCondition.Met)
                    {
                        p.Update();
                    }
                }

                activeList.Clear();
                activeList.AddRange(processList.Where(x => x.CurrentState != RunState.Done));

                processList.Clear();
                processList.AddRange(activeList);
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


}
