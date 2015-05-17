using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Atomic.Core;
using System.IO;

namespace WinConsole.Containers
{
    /*
    class FilesystemContainer : RunContainer, IDataDefinitionContainer
    {
        public override string ExecuteTask(ITask task)
        {
            IDictionary<string, object> response = new Dictionary<string, object>();
            string functionText = task.FunctionText.Trim();

            string[] tokens = functionText.Split(' ');

            switch (tokens[1])
            {
                case "directory":
                    ExecuteDirectoryFunction(response, functionText);
                    break;
                case "file":
                    ExecuteFileFunction(response, functionText);
                    break;
                default:
                    response["error"] = "unknown command";
                    response["function"] = functionText;
                    break;
            }

            return FormatResponse(response);
        }

        private string ExecuteDirectoryFunction(IDictionary<string, object> response, string functionText)
        {
            string[] tokens = functionText.Split(' ');
            string path = functionText.Substring(tokens[0].Length + tokens[1].Length + 2);
            // use Directory here - static class

            // use directory info in Directory process
            //DirectoryInfo dirInfo = new DirectoryInfo(path);

           
            /*
             * Also look at Directory...
            dirInfo.Create;
            dirInfo.Delete;
            dirInfo.MoveTo;
             * 
             * // in directory process
            dirInfo.Attributes;
            dirInfo.CreationTime;
            dirInfo.EnumerateDirectories;
            dirInfo.EnumerateFiles;
            dirInfo.Exists;
            dirInfo.FullName;
            dirInfo.LastAccessTime;
            dirInfo.LastWriteTime;
             * 
            // dest is only a subdir of current dir
            return "";
        }

        private string ExecuteFileFunction(IDictionary<string, object> response, string functionText)
        {
            string[] tokens = functionText.Split(' ');
            string path = functionText.Substring(tokens[0].Length + tokens[1].Length + 2);
            // use File here..

            // use FileInfo in FileProcess
            //FileInfo fileInfo = new FileInfo(path);

            //Environment.CurrentDirectory;
            // Path.getXXX() used for create temp files...
            /*
             * Also look at "File" class...
            fileInfo.CopyTo();
            fileInfo.Create();
            fileInfo.Decrypt();
            fileInfo.Delete();
            fileInfo.Encrypt();
            fileInfo.MoveTo;
            fileInfo.Replace;
             * // in file process
            fileInfo.AppendText();
            fileInfo.Attributes();
            fileInfo.CreateText();
            fileInfo.CreationTime();
            fileInfo.Directory;
            fileInfo.Exists();
            fileInfo.Extension;
            fileInfo.FullName;
            fileInfo.IsReadOnly;
            fileInfo.LastAccessTime;
            fileInfo.LastWriteTime;
            fileInfo.Length;
            fileInfo.Open;
            
            // create, // delete, // list
            // file can only be created in a subdir, unless temporary is specified
            return "";
        }

        public override void HandleError(string errorText)
        {
            throw new NotImplementedException();
        }

        public void Modify(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void Create(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void Delete(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void Move(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void Copy(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void List(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void Open(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void Close(IMessage message)
        {
            throw new NotImplementedException();
        }
    }
     * */
}
