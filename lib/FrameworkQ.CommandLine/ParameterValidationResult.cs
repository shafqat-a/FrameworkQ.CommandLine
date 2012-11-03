using System;
using System.Collections.Generic;
using System.Text;

namespace FrameworkQ.CommandLine
{
    class ParameterValidationResult
    {
        private string _errorMessage = string.Empty;
        private bool _isValid = false;

        public ParameterValidationResult()
        {
            _isValid = true;
        }

        public ParameterValidationResult(string errorMessage)
        {
            _isValid = false;
            _errorMessage = errorMessage;
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        public bool IsValid
        {
            get { return _isValid; }
            set { _isValid = value; }
        }
    }
}
