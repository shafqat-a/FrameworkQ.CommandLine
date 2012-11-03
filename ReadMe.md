What it does
------------
A simple but quite powerfull library for processing command line parameters.

Location
---------
The library is located at  

    /lib/FrameworkQ.CommandLine 

Demo application that shows usage

    /tests/FrameworkQ.CommandLine.Demo

Sample usage
------------

Create a command class for each command 
Then initialize the parser like this

    Parser parser = new Parser("Utility to zip/unzip files");
    parser.RegisterCommand(typeof(ZipCommand));
    parser.RegisterCommand(typeof(QuitCommand));
    parser.RegisterCommand(typeof(UnzipCommand));
    parser.IsFirstWordCommand = true;
    while (true)
    {
        Console.Write("Enter Command in the line below: \n");
        string input = Console.ReadLine();
        parser.CommandKeyword = CommandOption.Opt("action", CommandOption.CommandOptionType.String, false);
        CommandParseResult result = parser.ParseCommand(input.Split(" ".ToCharArray()));
        if (result.ErrorOccurred)
        {
            Console.WriteLine(result.ErrorMessage);
        }
        else
        {
            result.Command.Execute();
        }
    }
