using System;

namespace AspNetInsight.Installer
{
    internal class RemoveFromIisSite : InstallerCommand
    {
        public override string OptionText => "/rs";

        public override string Description => "/rs - Remove configuration for the given local IIS site.";
        public override bool ValidateParam(string[] parameter)
        {
            if (parameter == null)
                return false;

            var trm = parameter.TrimAll();
            return trm == null ? false :
                     trm.Length == 1;
        }

        public override Action<object> Exec => (values) =>
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            var trm = ((string[])values).TrimAll();

            Common.UpdateLocalIis(trm[0], true);
        };
    }
}