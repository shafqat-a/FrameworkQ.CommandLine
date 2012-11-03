using System;
using System.Collections.Generic;
using System.Text;

namespace FrameworkQ.CommandLine.Demo
{
    [Command ("quit", "Exits processing")]
    class QuitCommand  : Command
    {
        public QuitCommand()
            : base("quit")
        {
        }

        
        
        public override void Execute()
        {
            
            Environment.Exit(1);
        }
    }
}
