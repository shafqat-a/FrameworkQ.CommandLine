using System;
using System.Collections.Generic;
using System.Text;

namespace FrameworkQ.CommandLine
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        private string _commandName = string.Empty;
        private string _commandDescription = string.Empty;

        /// <summary>
        /// Takes the command name and the command description
        /// </summary>
        /// <param name="commandName"></param>
        /// <param name="commandDescription"></param>
        public CommandAttribute(string commandName, string commandDescription)
        {
            _commandName = commandName;
            _commandDescription = commandDescription;
        }

        public string CommandName
        {
            get { return _commandName; }
        }

        public string CommandDescription
        {
            get { return _commandDescription; }
        }
    }
}
