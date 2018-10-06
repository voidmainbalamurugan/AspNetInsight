using System;

namespace AspNetInsight.Installer
{
    internal class RemoveAll : InstallerCommand
    {
        public override string OptionText => "/u";

        public override string Description => "/u - Remove from local IIS, GAC and delete Datafile.";
        public override bool ValidateParam(string[] parameter) => !parameter.HasValue();

        public override Action<object> Exec => (values) =>
        {
            Common.RemoveAssembly(Common._assemblyName, Common._extn);
            Common.UpdateLocalIis(removeInstalled: true);
        };
    }
}