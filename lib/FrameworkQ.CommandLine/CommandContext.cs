using System;
using System.Collections.Generic;
using System.Text;

namespace FrameworkQ.CommandLine
{
    public class CommandContext
    {
        public Dictionary<string, string> ConfigurationDefaults
        {
            get;
            internal set;
        }
    }
}
