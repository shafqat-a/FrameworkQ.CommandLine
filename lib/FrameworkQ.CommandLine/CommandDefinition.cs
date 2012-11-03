using System;
using System.Collections.Generic;
using System.Text;

namespace FrameworkQ.CommandLine
{
    public class CommandDefinition
    {
        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private Type _commandType = null;
        public Type CommandType 
        {
            get {return _commandType;}
            set {_commandType = value;}
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private Dictionary<string, CommandOption> _options = new Dictionary<string, CommandOption>();
        public Dictionary<string, CommandOption> Options
        {
            get { return _options; }
        }

        public static CommandDefinition Define(string commandName, Type type)
        {
            CommandDefinition def = new CommandDefinition();
            def.Name = commandName;
            def.CommandType = type;
            return def;
        }
    }
}
