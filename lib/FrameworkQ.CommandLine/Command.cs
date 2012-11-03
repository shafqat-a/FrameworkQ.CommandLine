using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FrameworkQ.CommandLine
{
    /// <summary>
    /// Base class for any command
    /// </summary>
    public abstract class Command
    {
        public Command(string commandName)
        {
        }

        private string _commandName = string.Empty;
        /// <summary>
        /// Name of the commnad
        /// </summary>
        public string CommandName
        {
            get { return _commandName; }
        }

        private Dictionary<string, string> _parameters = new Dictionary<string, string>();
        /// <summary>
        /// String and value pair for the parameters
        /// </summary>
        public Dictionary<string, string> Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        /// <summary>
        /// Is this a path? 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected static bool IsPath(string element)
        {
            if (File.Exists(element) || Directory.Exists(element))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Is this part of some path?
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected static bool IsPathPart(string element)
        {
            return false;
        }

        public CommandContext Context
        {
            get;
            internal set;
        }

        /// <summary>
        /// Methods to be overridden by any command
        /// </summary>
        public abstract void Execute();
    }
}
