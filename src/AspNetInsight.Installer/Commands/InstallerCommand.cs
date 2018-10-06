using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AspNetInsight.Installer
{
    internal abstract class InstallerCommand
    {
        public abstract string OptionText { get; }
        public abstract string Description { get; }
        private static List<InstallerCommand> _commands { get; set; }
        private static List<string> _helperText = new List<string>()
        {
            string.Format("Installs {0} HttpModule into GAC and configure local IIS (7.0 and above).", Common._assemblyName),
            "\n" + string.Format("{0} /[option] [param]", Common._installerName)
        };

        public abstract Action<object> Exec { get; }
        
        public abstract bool ValidateParam(string[] parameter);

        public static InstallerCommand GetCommand(string option)
        {
            if (_commands == null)
                Init();

            var c = _commands.FirstOrDefault(cmd =>
                cmd.OptionText.Equals(option, StringComparison.InvariantCultureIgnoreCase));

            if (c == null)
                c = _commands.FirstOrDefault(cmd =>
                    cmd is HelperText);

            return c;
        }

        static InstallerCommand()
        {
            Init();
        }

        private static void Init()
        {
            _commands = new List<InstallerCommand>();
            var cmds = Assembly.GetExecutingAssembly().GetTypes().Where(
                t => t.BaseType.Name == "InstallerCommand" && !t.IsAbstract);
            foreach (var cmd in cmds)
            {
                var inst = Activator.CreateInstance(cmd);
                if (inst != null)
                    _commands.Add(inst as InstallerCommand);
            }
        }

        protected static List<string> GetAllCommandDesc()
        {
            return _helperText.Concat(_commands.ConvertAll(cmd => cmd.Description)).ToList();
        }
    }
}