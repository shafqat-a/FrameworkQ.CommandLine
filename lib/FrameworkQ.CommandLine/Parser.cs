using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace FrameworkQ.CommandLine
{
    public class Parser
    {
        public Parser(string whatDoesThisDo)
        {
            _whatDoesThisDo = whatDoesThisDo;
        }

        private Dictionary<string, string> _configurationDefaults = new Dictionary<string, string>();
        public Dictionary<string, string> ConfigurationDefaults
        {
            get { return _configurationDefaults; }
        }

        public bool UseConfigurationDefaults
        {
            get;
            set;
        }

        #region Properties
        private string _parameterSpecifier = "/"; 

        /// <summary>
        /// Specifies which character specifies parameters. Usually "/" or "-"
        /// </summary>
        public string ParameterSpecifier
        {
            get { return _parameterSpecifier; }
            set { _parameterSpecifier = value; }
        }

        private string _parameterValueStarter = " ";
        
        /// <summary>
        /// Which character after the paramter marks the start of the parameter. For example 
        /// /parmeter value, or /parameter:value
        /// </summary>
        public string ParameterValueStarter
        {
            get { return _parameterValueStarter; }
            set { _parameterValueStarter = value; }
        }

        private Dictionary<string, CommandDefinition> _commands = new Dictionary<string, CommandDefinition>();
        /// <summary>
        /// Contains definitions for the commands
        /// </summary>
        public Dictionary<string, CommandDefinition> CommandDefinitions
        {
            get { return _commands; }
        }


        private CommandOption _commandKeyword = null;
        /// <summary>
        /// Specifies the command keyword. if there is no command like keyword then 
        /// the command system much have set IsFirstWordCommand to true. 
        /// This framework supports multiple commands like the subversion.
        /// <example>
        /// If you have a compress utility and have two commands zip, unzip and use 
        /// the word action to mark it then the command keyword should be action and the
        /// usage would be like : <para>
        /// "compress /action zip" or "compress /action unzip" 
        /// </para>
        /// </example>
        /// </summary>
        public CommandOption CommandKeyword
        {
            get { return _commandKeyword; }
            set { _commandKeyword = value; }
        }

        private bool _isFirstWordCommand = false;
        /// <summary>
        /// If the first word is the keyword then the command keyword is ignored and the first
        /// word is treated as the specified command
        /// </summary>
        public bool IsFirstWordCommand
        {
            get { return _isFirstWordCommand; }
            set { _isFirstWordCommand = value; }
        }

        #endregion

        /// <summary>
        /// Gets the parameter values as a single string
        /// </summary>
        /// <param name="paramTokens"></param>
        /// <returns></returns>
        private string GetParameterValue(string[] paramTokens)
        {
            StringBuilder sb = new StringBuilder();
            if (paramTokens.Length > 1)
            {
                for (int i = 1; i < paramTokens.Length; i++)
                {
                    sb.Append(paramTokens[i]);
                    if (i < paramTokens.Length - 1)
                    {
                        sb.Append(this.ParameterValueStarter);
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Parses the command line arguments and returns a command parse result
        /// </summary>
        /// <param name="argv"></param>
        /// <returns></returns>
        public CommandParseResult ParseCommand(string[] argv)
        {
            // Make the whole string array into one single line
            string commandLine = string.Join(" ", argv);
            // Try to split the string into parameters
            string[] parms = commandLine.Split (this.ParameterSpecifier.ToCharArray());

            Dictionary<string, string> commandParts = new Dictionary<string, string>();

            StringBuilder sbError = new StringBuilder();
            
            // Try to process the tokens
            foreach (string token in parms)
            {
                // Check if the parameter we are trying to read is valid or not
                if (!string.IsNullOrEmpty(token))
                {
                    // Since both the parameter name and the parameter value are in this string 
                    // lets try to seperate the parameter name and the parameter values. Also please 
                    // note that when the Parameter Value starter can be present multiple times in 
                    // the command. For example if space is the patameter starter value then it can 
                    // bre present multiple times. but we will take the first instance and count 
                    // parameter values from there. See example below
                    // parameter name : path
                    // parameter specifier : "/"
                    // parameter value starter : " " (space)
                    // The string is like "/path c:\program files\test.txt" 
                    // In this case the parameter needs to eb counted from 6th position
                    string[] tokenParts = token.Split(this.ParameterValueStarter.ToCharArray());
                    if (!string.IsNullOrEmpty(tokenParts[0]))
                    {
                        // take only the first part
                        commandParts[tokenParts[0].Trim()] = GetParameterValue(tokenParts).Trim();
                    }
                }
            }

            string commandString = string.Empty;
            if (IsFirstWordCommand)
            {
                // figure out the first word that should serve as the command
                // Find out the first iteration of the 
                // Says the first word is the command ... so there is no keyword required. That means 
                // instead of executable.exe /keyword command /option value ...
                // the command will look like
                // instead of executable.exe command /option value ...

                int parmStart = commandLine.IndexOf(this.ParameterSpecifier);
                if (parmStart > 1)
                {
                    commandString = commandLine.Substring(0, parmStart).Trim();
                }
                else
                {
                    commandString = commandLine.Trim();
                }
            }
            
            Command cmd = null;
            bool isParameterError = false;
            // Since we have parsed all command parts, we can now go and build the command
            if (commandParts.ContainsKey(this.CommandKeyword.Name) || commandString.Length>0)
            {
                // Well we have found a command. We need to verify and validate if all items in
                // the command were specified as needed

                if (!IsFirstWordCommand)
                {
                    commandString = this.CommandKeyword.Name;
                }

                string validationErrors = ValidateCommandOptions(commandParts, commandString);
                sbError.Append(validationErrors);
                if (string.IsNullOrEmpty(validationErrors))
                {
                    cmd = GetCommand(commandParts, commandString);
                    if (cmd == null)
                    {
                        sbError.Append("Invalid command");
                    }
                }
                else
                {
                    isParameterError = true;
                    sbError.AppendLine(GetCommandHelpString(this.CommandDefinitions[commandString]));
                    sbError.AppendLine(validationErrors);
                }
            }
            else
            {
                sbError.AppendFormat("Error: {0} was not specified\n", IsFirstWordCommand ? "Command" : this.CommandKeyword.Name);
            }

            if (cmd == null)
            {
                if (!isParameterError)
                {
                    sbError.AppendLine(GetHelpString());
                }
            }

            string errorString = sbError.ToString();
            CommandContext context = new CommandContext() { ConfigurationDefaults = this.ConfigurationDefaults };
            return new CommandParseResult(cmd, errorString.Length>0 ? true: false, errorString);
        }

        private Command GetCommand(Dictionary<string, string> dicCommandParts, string commandName)
        {
            Command cmd = null;
            if (this.CommandDefinitions.ContainsKey(commandName))
            {
                cmd = Activator.CreateInstance(this.CommandDefinitions[commandName].CommandType) as Command;
                // Created the command object, now we need to pass the parameters
                // Lets go through the parameters and run the command
                PropertyInfo[] props = this.CommandDefinitions[commandName].CommandType.GetProperties();

                foreach (PropertyInfo prop in props)
                {
                    object[] attrs = prop.GetCustomAttributes
                        (typeof(CommandParameterAttribute), true);
                    if (attrs.Length > 0)
                    {
                        CommandParameterAttribute attr = (CommandParameterAttribute)attrs[0];
                        string optionName = attr.CommandParameter;

                        if (dicCommandParts.ContainsKey(optionName))
                        {
                            CommandOption opt = this.CommandDefinitions[commandName].Options[optionName];
                            if (opt.OptionType == CommandOption.CommandOptionType.Boolean)
                            {
                                prop.SetValue(cmd, opt.GetBoolValue(dicCommandParts[optionName]), (object[])null);
                            }
                            else if (opt.OptionType == CommandOption.CommandOptionType.Integer)
                            {
                                prop.SetValue(cmd, opt.GetIntValue(dicCommandParts[optionName]), (object[])null);
                            }
                            else if (opt.OptionType == CommandOption.CommandOptionType.String ||
                                opt.OptionType == CommandOption.CommandOptionType.DirectoryPath ||
                                opt.OptionType == CommandOption.CommandOptionType.FilePath||
                                opt.OptionType == CommandOption.CommandOptionType.PossibleFilePath ||
                                opt.OptionType == CommandOption.CommandOptionType.StringSet)
                            {
                                prop.SetValue(cmd, dicCommandParts[optionName], (object[])null);
                            }
                        }
                        else
                        {
                        }
                    }
                }
            }
            return cmd;
        }

        /// <summary>
        /// Validates the command string and checks if required parameters were specified.
        /// Returns any error as string.
        /// </summary>
        /// <param name="dicCommandParts"></param>
        /// <param name="commandName"></param>
        /// <returns></returns>
        private string ValidateCommandOptions(Dictionary<string, string> dicCommandParts, string commandName)
        {
            StringBuilder sbError = new StringBuilder();
            if (this.CommandDefinitions.ContainsKey(commandName))
            {
                // Try to find the command definition.
                if (this.CommandDefinitions.ContainsKey(commandName))
                {
                    CommandDefinition def = this.CommandDefinitions[commandName];
                    //bool commandIsValid = true;
                    foreach (CommandOption opt in def.Options.Values)
                    {
                        // Validate that all required parameters have been passed
                        if (dicCommandParts.ContainsKey(opt.Name))
                        {
                            ParameterValidationResult result = opt.IsValidValue(dicCommandParts[opt.Name]);
                            if (!result.IsValid)
                            {
                                sbError.AppendFormat("Value for {0} was not valid.\n{1}", opt.Name, result.ErrorMessage);
                                return sbError.ToString();
                            }
                        }
                        else
                        {
                            if (!opt.IsOptional)
                            {
                                if (this.UseConfigurationDefaults)
                                {
                                    if (this.ConfigurationDefaults.ContainsKey(opt.Name))
                                    {
                                        ParameterValidationResult result = opt.IsValidValue(ConfigurationDefaults[opt.Name]);
                                        if (!result.IsValid)
                                        {
                                            sbError.AppendFormat("Value for {0} was not provided and the default value in config was not valid.\n{1}", opt.Name, result.ErrorMessage);
                                            return sbError.ToString();
                                        }
                                        else
                                        {
                                            dicCommandParts[opt.Name] = ConfigurationDefaults[opt.Name];
                                        }
                                    }
                                    else
                                    {
                                        sbError.AppendFormat("Value for {0} is required.", opt.Name);
                                        return sbError.ToString();
                                    }
                                }
                                else
                                {
                                    sbError.AppendFormat("Value for {0} is required.", opt.Name);
                                    return sbError.ToString();
                                }
                            }
                        }
                    }
                    return sbError.ToString();
                }
            }
            return sbError.ToString();
        }

        /// <summary>
        /// Registers a command given that the command has command attribute and
        /// parameters attributes
        /// </summary>
        /// <param name="typCommand"></param>
        /// <returns></returns>
        public bool RegisterCommand(Type typCommand)
        {
            CommandDefinition cd = new CommandDefinition();

            object[] attrs = typCommand.GetCustomAttributes(typeof(CommandAttribute), true);
            if (attrs.Length > 0)
            {
                // Create the command definition
                CommandAttribute cmdAttr = attrs[0] as CommandAttribute;
                cd.Name = cmdAttr.CommandName;
                cd.CommandType = typCommand;
                cd.Description = cmdAttr.CommandDescription;

                // Now create the command parameters
                PropertyInfo[] pInfos = typCommand.GetProperties();
                foreach (PropertyInfo pinfo in pInfos)
                {
                    attrs = pinfo.GetCustomAttributes(typeof(CommandParameterAttribute), true);
                    if (attrs.Length > 0)
                    {
                        CommandParameterAttribute parmAttr = attrs[0] as CommandParameterAttribute;
                        CommandOption opt = CommandOption.Opt(parmAttr.CommandParameter,
                            parmAttr.OptionType, parmAttr.IsOptional);
                        opt.AdditionalHelp = parmAttr.AdditionalHelpText;
                        cd.Options.Add(opt.Name, opt);
                    }
                }

                this.CommandDefinitions.Add(cd.Name, cd);

                return true;
            }
            return false;
        }

        public bool UnRegisterCommand(string commandName)
        {
            if (this.CommandDefinitions.ContainsKey(commandName))
            {
                this.CommandDefinitions.Remove(commandName);
                return true;
            }
            return false;
        }

        private string _whatDoesThisDo = string.Empty;
        public string WhatDoesThisDo
        {
            get { return _whatDoesThisDo; }
        }

        public string GetHelpString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("\n\n{0}\n",this.WhatDoesThisDo);
            FileInfo fi = new FileInfo ( Environment.GetCommandLineArgs()[0]);
            sb.AppendFormat("Usage: {0} {1}{2}{3}command",fi.Name,
                this.ParameterSpecifier, this.CommandKeyword.Name, this.ParameterValueStarter);
            sb.AppendLine ("\n\nAvailable commands are ...");
            foreach (CommandDefinition cd in this.CommandDefinitions.Values)
            {
                sb.AppendLine (string.Format("{0}:\t{1}", cd.Name, cd.Description));
            }
            return sb.ToString();
        }

        private string GetCommandHelpString(CommandDefinition cmdDef)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("\n\n{0}:\t{1}\n", cmdDef.Name, cmdDef.Description);
            sb.AppendFormat("\nParameters for {0}\n------------------\n", cmdDef.Name);
            foreach (CommandOption opt in cmdDef.Options.Values)
            {
                if (opt.IsOptional)
                {
                    sb.AppendFormat("[{0}]:\t{1}\n", opt.Name, opt.AdditionalHelp);
                }
                else
                {
                    sb.AppendFormat("{0}:\t{1}\n", opt.Name, opt.AdditionalHelp);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Finds out the command to be executed
        /// </summary>
        /// <returns></returns>
        private string GetCommandString(Dictionary<string, string> commandParts, string[] args)
        {
            string commandText = string.Empty;
            
            if (IsFirstWordCommand)
            {
                foreach (string arg in args)
                {
                    if (!string.IsNullOrEmpty(arg))
                    {
                        return arg;
                    }
                }
            }
            else
            {
            }
            return commandText;
        }

    }
}
