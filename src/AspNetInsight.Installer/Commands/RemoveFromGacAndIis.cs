using System;

namespace AspNetInsight.Installer
{
    internal class RemoveFromGacAndIis : InstallerCommand
    {
        public override string OptionText => "/r";

        public override string Description => "/r - Remove from local IIS and GAC.";
        public override bool ValidateParam(string[] parameter) => !parameter.HasValue();

        public override Action<object> Exec => (values) =>
        {
            Common.RemoveAssembly(Common._assemblyName, Common._extn);
            Common.UpdateLocalIis(removeInstalled: true);
        };
    }
}