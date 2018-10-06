using System;

namespace AspNetInsight.Installer
{
    internal class GacInstaller : InstallerCommand
    {
        public override string OptionText => "/i";

        public override string Description => "/i - Install HttpModule into GAC.";

        public override bool ValidateParam(string[] parameter) => !parameter.HasValue();

        public override Action<object> Exec => (values) =>
        {
            Common.InstallAssembly(Common._assemblyName, Common._extn);
        };
    }
}