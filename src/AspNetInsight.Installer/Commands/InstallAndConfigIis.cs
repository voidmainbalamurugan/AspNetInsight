using System;

namespace AspNetInsight.Installer
{
    internal class InstallAndConfigIis : InstallerCommand
    {
        public override string OptionText => "/ic";
        public override string Description => "/ic - Install HttpModule into GAC and configure the local IIS sites.";

        public override bool ValidateParam(string[] parameter) => !parameter.HasValue();

        public override Action<object> Exec => (values) =>
        {
            Common.Install();
        };
    }
}