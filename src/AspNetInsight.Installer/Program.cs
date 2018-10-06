using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace AspNetInsight.Installer
{
    class Program
    {
        const string _defaultOption = "/i";
        const string _helperOption = "/?";
        public static bool _logToFile = true;
        public static string _logFileName = "_AspNetInsight.Installer.log";
        
        static void Main(string[] args)
        {
            Setup();

            try
            {
                RunInstaller(args);
            }
            catch(Exception ex)
            {
                ex.WriteToLog();
                "Error occurred, Please refer the log file for more information!.\n".WriteToConsole();
            }
            finally
            {
                "\nCompleted! Please any key to exit.".WriteToConsole();

            }
            Console.ReadKey();
        }

        static void RunInstaller(string[] args)
        {
            if (Common.IsAdmin())
            {
                if (args.Length == 0)
                    InstallerCommand.GetCommand(_defaultOption).Exec(null);
                else
                {
                    var cmd = InstallerCommand.GetCommand(args[0]);

                    var param = args.Length > 1 ? args.Skip(1).ToArray() : null;
                    if (cmd.ValidateParam(param))
                        cmd.Exec(param);
                    else
                    {
                        "Invalid option specified !\n".WriteToConsole();
                        InstallerCommand.GetCommand(_helperOption).Exec(null);
                    }
                }
            }
            else
                "Installation requires Admin rights!".WriteToConsole();
        }

        static void Setup()
        {
            if (_logToFile)
            {
                var lstnr = new TextWriterTraceListener(_logFileName);
                lstnr.TraceOutputOptions = TraceOptions.DateTime;
                Trace.Listeners.Add(lstnr);
                Trace.AutoFlush = true;
                Trace.IndentSize = 5;
            }
        }
    }
}
