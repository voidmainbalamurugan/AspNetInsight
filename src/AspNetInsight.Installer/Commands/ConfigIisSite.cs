using System;

namespace AspNetInsight.Installer
{
    internal class ConfigIisSite : InstallerCommand
    {
        public override string OptionText => "/cs";

        public override string Description => "/cs - Configure local IIS site by given site name.";
        public override bool ValidateParam(string[] parameter)
        {
            if (parameter == null)
                return false;

            var trm = parameter.TrimAll();
            return trm != null && trm.Length == 1;
        }

        public override Action<object> Exec => (values) =>
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            var trm = ((string[])values).TrimAll();
            Common.UpdateLocalIis(trm[0]);
        };
    }
}