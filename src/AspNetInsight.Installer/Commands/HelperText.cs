using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetInsight.Installer
{
    internal class HelperText : InstallerCommand
    {
        public override string OptionText => "/?";

        public override string Description => "";
        private static List<string> _example = 
            new List<string>()
            {
                "\n EXAMPLE-(1): " + string.Format("{0} /i", Common._installerName),
                "\n EXAMPLE-(2): " + string.Format("{0} /cs 'Default Web Site'", Common._installerName)
            };

        public override bool ValidateParam(string[] parameter) => true;

        public override Action<object> Exec => (values) =>
        {
            string.Join(Environment.NewLine, GetAllCommandDesc().Concat(_example)).WriteToConsole();
        };
    }
}