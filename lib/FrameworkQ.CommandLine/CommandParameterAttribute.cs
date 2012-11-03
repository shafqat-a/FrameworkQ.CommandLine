using System;
using System.Collections.Generic;
using System.Text;

namespace FrameworkQ.CommandLine
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CommandParameterAttribute : System.Attribute
    {
        // Parameter name 
        private string _commandParameter = string.Empty;
        // Default type is string
        private CommandOption.CommandOptionType _optionType = CommandOption.CommandOptionType.String;
        bool _isOptional= false;
        bool _useConfigurationDefualts = false;
        private string _additionalHelpText = string.Empty;

        public CommandParameterAttribute(string commandParameter)
            : this(commandParameter, false, CommandOption.CommandOptionType.String,false, string.Empty)
        {
        }

        public CommandParameterAttribute(string commandParameter, string addtionalHelpText)
            : this(commandParameter, false, CommandOption.CommandOptionType.String, false, addtionalHelpText)
        {
        }

        public CommandParameterAttribute(string commandParameter, CommandOption.CommandOptionType optionType)
            : this(commandParameter, false, optionType, false, string.Empty)
        {
        }

        public CommandParameterAttribute(string commandParameter, bool isOptional, 
            CommandOption.CommandOptionType optionType, bool useConfigurationDefaults, 
            string additionalHelpText)
        {
            _commandParameter = commandParameter;
            _isOptional = isOptional;
            _optionType = optionType;
            _additionalHelpText = additionalHelpText;
            _useConfigurationDefualts = useConfigurationDefaults;

        }

        public string CommandParameter
        {
            get { return _commandParameter; }
        }

        public bool UseConfigurationDefualts
        {
            get { return _useConfigurationDefualts; }
        }



        public bool IsOptional
        {
            get { return _isOptional; }
        }

        public CommandOption.CommandOptionType OptionType
        {
            get { return _optionType; }
        }

        public string AdditionalHelpText
        {
            get { return _additionalHelpText; }
        }
    }
}
