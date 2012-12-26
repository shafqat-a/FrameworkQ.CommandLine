#What it does
A simple but quite powerfull library for processing command line parameters.

##Location
The library is located at  

    /lib/FrameworkQ.CommandLine 

Demo application that shows usage

    /tests/FrameworkQ.CommandLine.Demo

##Example 

Lets assume that we want to create an application that either zips or unzips files. Lets think that the zip command is used like this 

    zip -in [files to zip] -out [output zip file name]

also lets have a unzip command that looks like this 

    unzip -in [file to unzip] -out [path to extract the files to]

For each command create a command class, which may look like this 

        [Command ("zip", "Compresses the file")]
        class ZipCommand : Command
        {
            private string _in = string.Empty;
            private string _outpath = string.Empty;    
            
            public ZipCommand(): base("zip") {}
            
            [CommandParameter("in", false, 
                CommandOption.CommandOptionType.FilePath,false,
                "The file to be zipped")]
            public string In
            {
                get { return _in; }
                set { _in = value; }
            }    
            [CommandParameter("out", false, 
                CommandOption.CommandOptionType.PossibleFilePath, false,
                "Output zip file name")]
            public string Outpath
            {
                get { return _outpath; }
                set { _outpath = value; }
            }
    
            private int _bufferSize = 512;
            [CommandParameter("buffer", true, CommandOption.CommandOptionType.Integer,
                true, "Number of bytes to zip on each iteration")]
            public int BufferSize
            {
                get { return _bufferSize; }
                set { _bufferSize = value; }
            }
    
            [System.Security.Permissions.FileIOPermission(
                System.Security.Permissions.SecurityAction.Demand)]
            public override void Execute()
            {
                if (File.Exists(this.In))
                {
                    /*  The actual zipping code */
                }
            }
        }

Create similar classes for `unzip` command

Now you can just launch the command line with the following code.

    Parser parser = new Parser("Utility to zip/unzip files");
    parser.RegisterCommand(typeof(ZipCommand));
    parser.RegisterCommand(typeof(UnzipCommand));
    parser.IsFirstWordCommand = true;

    string input = string.Join(" ", args);
    parser.CommandKeyword = CommandOption.Opt("action", 
        CommandOption.CommandOptionType.String, false);
    CommandParseResult result = parser.ParseCommand(input.Split(" ".ToCharArray()));
    if (result.ErrorOccurred)
    {
        Console.WriteLine(result.ErrorMessage);
    }
    else
    {
        result.Command.Execute();
    }

Lets assume that the output executable is called `demo.exe`

Now if we enter `demo.exe` in command line the output is :
    
    Error: Command was not specified
        
    Utility to zip/unzip files
    Usage: demo /action command
    
    Available commands are ...
    zip:    Compresses the file
    quit:   Exits processing
    unzip:  Compresses the file

Now if we enter `demo.exe zip` the output is : 

    Value for in is required.
    
    zip:    Compresses the file
    
    Parameters for zip
    ------------------
    in:     The file to be zipped
    out:    Output zip file name
    [buffer]:       Number of bytes to zip on each iteration
    
    Value for in is required.

Now if we enter `demo.exe /in c:\abc.html /out c:\abc.zip` then we will a nice zip file as output and the file will be zipped.

### Other uses

The above sample we could have written 

    parser.ParameterSpecifier = "-";
    
Then it would have to use `'-'` instead of `'/'` (default) for specifying parameters

So the command `demo.exe /in c:\abc.html /out c:\abc.zip` would look like 

    demo.exe -in c:\abc.html -out c:\abc.zip
    
Also space is teh default classifier for command parameter value, we could replace that with the following code 

    parser.ParameterValueStarter = ':'
    
Now we would have to use 

    demo.exe -in:c:\abc.html -out:c:\abc.zip
    
Have fun. Never write command line parsing code again.

