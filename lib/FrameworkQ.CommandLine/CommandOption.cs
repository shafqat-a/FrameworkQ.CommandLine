using System;
using System.Collections.Generic;
using System.Text;

namespace FrameworkQ.CommandLine
{
    public class CommandOption
    {
        /// <summary>
        /// Type of parameter a command takes
        /// </summary>
        public enum CommandOptionType
        {
            /// <summary>
            /// The parameter is a string
            /// </summary>
            String,
            /// <summary>
            /// The parameter is an int
            /// </summary>
            Integer,
            /// <summary>
            /// The parameter expects boolean "y","n","yes","no" also ok
            /// </summary>
            Boolean,
            /// <summary>
            /// Accepts any value from a set of strings
            /// </summary>
            StringSet,
            /// <summary>
            /// Path to a folder that is valid
            /// </summary>
            DirectoryPath,
            /// <summary>
            /// Path a file that is valid
            /// </summary>
            FilePath,
            /// <summary>
            /// Path a file that is valid
            /// </summary>
            PossibleFilePath

        }


        /// <summary>
        /// Create a command option
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="isOptional"></param>
        /// <returns></returns>
        public static CommandOption Opt(string name, CommandOptionType type, bool isOptional)
        {
            CommandOption opt = new CommandOption();
            opt.Name = name;
            opt.OptionType = type;
            opt.IsOptional = isOptional;
            return opt;
        }

        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private CommandOptionType _optionType = CommandOptionType.String;
        public CommandOptionType OptionType
        {
            get { return _optionType; }
            set { _optionType = value; }
        }

        private bool _isOptional = false;
        public bool IsOptional
        {
            get { return _isOptional; }
            set { _isOptional = value; }
        }

        private string[] _stringSet = null;
        public string[] StringSet
        {
            get { return _stringSet; }
            set { _stringSet = value; }
        }

        private string _additionalHelp = string.Empty;
        public string AdditionalHelp
        {
            get { return _additionalHelp; }
            set { _additionalHelp = value; }
        }

        internal int GetIntValue(string stringValue)
        {
            int ret = 0;
            if (int.TryParse(stringValue, out ret))
            {
                return ret;
            }
            return 0;
        }

        internal Path GetPathValue(string stringValue)
        {
            return null;
        }

        internal static bool ParseBoolean(string stringValue, out bool parsed)
        {
            if (bool.TryParse(stringValue, out parsed))
            {
                return true;
            }
            else
            {
                if (stringValue.Equals("y", StringComparison.InvariantCultureIgnoreCase))
                {
                    parsed = true;
                    return true;
                }
                if (stringValue.Equals("n", StringComparison.InvariantCultureIgnoreCase))
                {
                    parsed = false;
                    return true;
                }

                if (stringValue.Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                {
                    parsed = true;
                    return true;
                }
                if (stringValue.Equals("no", StringComparison.InvariantCultureIgnoreCase))
                {
                    parsed = false;
                    return true;
                }

                if (stringValue.Equals("1"))
                {
                    parsed = true;
                    return true;
                }
                if (stringValue.Equals("0", StringComparison.InvariantCultureIgnoreCase))
                {
                    parsed = false;
                    return true;
                }

                return false;
            }

        }

        internal bool GetBoolValue (string stringValue)
        {
            bool ret = false;
            if (ParseBoolean(stringValue, out ret))
            {
                return ret;
            }
            return false;
        }

        internal ParameterValidationResult IsValidValue(string value)
        {
            ParameterValidationResult parmVal = null;
            if (this.OptionType == CommandOptionType.Boolean)
            {
                bool x = false;
                if (ParseBoolean(value, out x))
                {
                    return new ParameterValidationResult();
                }
                else
                {
                    return new ParameterValidationResult("Expects a boolean value such as true/false,yes/no,y/n");
                }
            }
            if (this.OptionType == CommandOptionType.Integer)
            {
                int x = 0;
                if (int.TryParse(value, out x))
                {
                    return new ParameterValidationResult();
                }
                else
                {
                    return new ParameterValidationResult("Expects an integer value");
                }
            }
            if (this.OptionType == CommandOptionType.String)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    return new ParameterValidationResult();
                }
                else
                {
                    return new ParameterValidationResult("Expects a string, but nothing was found");
                }
            }
            if (this.OptionType == CommandOptionType.StringSet)
            {
                foreach (string str in this.StringSet)
                {
                    if (string.Equals( str, value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return new ParameterValidationResult();
                    }
                }
                return new ParameterValidationResult("ERROR");
            }
            if (this.OptionType == CommandOptionType.DirectoryPath)
            {
                Path path = new Path();
                path.FolderOrFile = Path.PathType.FolderOnly;
                path.PathString = value;
                path.IsRelativePathAccepted = true;
                if (path.IsValidPath())
                {
                    return new ParameterValidationResult();
                }
                return new ParameterValidationResult ("Expects a valid directory path exists");
            }
            if (this.OptionType == CommandOptionType.FilePath)
            {
                Path path = new Path();
                path.FolderOrFile = Path.PathType.FileOnly;
                path.PathString = value;
                path.IsRelativePathAccepted = true;
                if (path.IsValidPath())
                {
                    return new ParameterValidationResult();
                }
                return new ParameterValidationResult ("Expects a valid file path that exists");
            }
            if (this.OptionType == CommandOptionType.PossibleFilePath)
            {
                Path path = new Path();
                path.FolderOrFile = Path.PathType.PossibleFilePath;
                path.PathString = value;
                path.IsRelativePathAccepted = true;
                if (path.IsValidPath())
                {
                    return new ParameterValidationResult();
                }
                return new ParameterValidationResult("Expects a valid path where atleast the path to the directory where the file remains exists");
            }
            throw new NotImplementedException();
        }

    }
}
