using System;
using System.Collections.Generic;
using System.Text;

namespace FrameworkQ.CommandLine
{
    public class CommandParseResult
    {
        private Command _command = null;
        public Command Command
        {
            get { return _command; }
        }

        private bool _errorOccurred = false;
        public bool ErrorOccurred
        {
            get { return _errorOccurred; }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        internal CommandParseResult(Command cmd, bool isError, string error)
        {
            _command = cmd;
            _errorOccurred = isError;
            _errorMessage = error;
        }
    }
}
