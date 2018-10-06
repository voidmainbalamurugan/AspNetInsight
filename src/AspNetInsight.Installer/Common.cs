using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using Microsoft.Web.Administration;
using System.EnterpriseServices.Internal;
using System.IO;
using System.Diagnostics;

namespace AspNetInsight.Installer
{
    public static class Common
    {
        public const string _httpModuleName = "si_ResponseTracker";
        public static string _assemblyName = "AspNetInsight4";
        public static string _extn = "dll";
        public const string _installerName = "consoleApp.exe";
        public const string _appPoolAttribute = "applicationPool";
        const string _classicPath = "system.web/httpModules";
        const string _integratedPath = "system.webServer/modules";

        public static string GetAppPoolName(this Site site)
        {
            return site?.Applications[0]?.Attributes[_appPoolAttribute].Value.ToString();
        }

        public static string GetModuleSectionName(this ManagedPipelineMode mode)
        {
            return mode == ManagedPipelineMode.Classic ?
                    _classicPath : _integratedPath;
        }

        static IEnumerable<ConfigurationElement> GetModules(this ConfigurationElementCollection modulesCollection, string moduleKey, string moduleName)
        {
            return modulesCollection.Where(mc =>
                mc.Attributes.Any(an =>
                    an.Name.Equals(moduleKey, StringComparison.InvariantCultureIgnoreCase)
                        && an.Value.ToString().Equals(moduleName, StringComparison.InvariantCultureIgnoreCase)
                        ));
        }

        public static void RemoveModule(this ConfigurationSection modulesSection, string moduleName)
        {
            if (modulesSection == null)
                throw new ArgumentNullException(nameof(moduleName));

            const string moduleKey = "name";
            var sections = modulesSection.GetCollection();
            var exists = sections.GetModules(moduleKey, moduleName);

            if (exists.Any())
            {
                foreach (var c in exists.ToList())
                {
                    sections.Remove(c);
                }
            }
        }

        public static void AddUpdateModule(this ConfigurationSection modulesSection, ApplicationPool appPool, string moduleName, Type type, bool overrideIfExists)
        {
            if (modulesSection == null)
                throw new ArgumentNullException(nameof(moduleName));

            const string moduleKey = "name";
            const string moduleTypeKey = "type";
            const string preconditionKey = "preCondition";
            const string managedHanlderValue = @"managedHandler";

            var sections = modulesSection.GetCollection();
            var exists = sections.GetModules(moduleKey, moduleName);

            ConfigurationElement addElement = null;
            if (!exists.Any())
                addElement = sections.CreateElement("add");
            else if (overrideIfExists)
                addElement = exists.FirstOrDefault();
            else
                return;

            addElement[moduleKey] = moduleName;

            addElement[moduleTypeKey] = string.Format("{0}, {1}", type.FullName, type.Assembly.FullName);
            if (appPool.ManagedPipelineMode == ManagedPipelineMode.Integrated)
            {
                addElement[preconditionKey] = managedHanlderValue;
            }

            if (!exists.Any())
                sections.Add(addElement);
        }

        /// <summary>
        /// reference:
        /// https://ayende.com/blog/158401/are-you-an-administrator
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static bool IsAdministrator(string username)
        {
            PrincipalContext ctx;
            try
            {
                Domain.GetComputerDomain();
                try
                {
                    ctx = new PrincipalContext(ContextType.Domain);
                }
                catch (PrincipalServerDownException)
                {
                    // can't access domain, check local machine instead 
                    ctx = new PrincipalContext(ContextType.Machine);
                }
            }
            catch (ActiveDirectoryObjectNotFoundException)
            {
                // not in a domain
                ctx = new PrincipalContext(ContextType.Machine);
            }
            var up = UserPrincipal.FindByIdentity(ctx, username);
            if (up != null)
            {
                PrincipalSearchResult<Principal> authGroups = up.GetAuthorizationGroups();
                return authGroups.Any(principal =>
                                      principal.Sid.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid) ||
                                      principal.Sid.IsWellKnown(WellKnownSidType.AccountDomainAdminsSid) ||
                                      principal.Sid.IsWellKnown(WellKnownSidType.AccountAdministratorSid) ||
                                      principal.Sid.IsWellKnown(WellKnownSidType.AccountEnterpriseAdminsSid));
            }
            return false;
        }

        internal static void WriteToConsole(this string value)
        {
            Console.WriteLine(value);
            if (Program._logToFile)
                WriteToLog(value);
        }

        internal static void WriteToLog(this string value)
        {
            Trace.TraceInformation(value);
        }

        internal static void WriteToLog(this Exception error)
        {
            Trace.TraceError(error.GetTrace());
        }

        static string GetTrace(this Exception error)
        {
            return string.Format("[Msg: {0}, Type: {1}, StackInfo: {2}, Source: {3}, InnerEx: {4}]", 
                error.Message,
                error.GetType().FullName,
                error.StackTrace?? "",
                error.Source?? "",
                error.InnerException?.GetTrace()
                );
        }

        internal static void Install()
        {
            InstallAssembly(_assemblyName, _extn);
            UpdateLocalIis();
        }

        internal static bool IsAdmin()
        {
            var name = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            var isAdmin = Common.IsAdministrator(name);
            return isAdmin;
        }

        internal static void InstallAssembly(string name, string ext)
        {
            var path = RemoveAssembly(name, ext);
            ("Installing into gac: " + name).WriteToConsole();
            new Publish().GacInstall(path);
        }

        internal static string RemoveAssembly(string name, string ext)
        {
            var path = CreateAssembly(name, ext);
            new Publish().GacRemove(path);
            ("Removed from gac: " + name).WriteToConsole();

            return path;
        }

        internal static string CreateAssembly(string name, string ext)
        {
            var rnm = typeof(Program).Assembly.GetManifestResourceNames();
            var rname = rnm.FirstOrDefault(n => n.EndsWith(string.Format("{0}.{1}", name, ext), StringComparison.InvariantCultureIgnoreCase));
            var dll = string.Format("\\{0}.dll", name);
            var path = Environment.CurrentDirectory + dll;

            using (var st = typeof(Program).Assembly.GetManifestResourceStream(rname))
            {
                var byt = new byte[st.Length];
                st.Read(byt, 0, (int)st.Length);
                File.WriteAllBytes(path, byt);
            }
            return path;
        }

        internal static void InstallModule(string siteName, string moduleName, Type type, bool overrideIfExists, bool remove = false)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                var site = serverManager.Sites[siteName];
                var config = site.GetWebConfiguration();
                var appPool = serverManager
                    .ApplicationPools[site.GetAppPoolName()];
                var sec = appPool.ManagedPipelineMode.GetModuleSectionName();

                if (remove)
                    config.GetSection(sec).RemoveModule(moduleName);
                else
                    config.GetSection(sec).AddUpdateModule(appPool, moduleName, type, overrideIfExists);

                serverManager.CommitChanges();
            }
        }

        internal static List<Site> GetSitesByName(string siteName, string managedVersion = "v4.0")
        {
            IEnumerable<Site> result = null;
            using (ServerManager serverManager = new ServerManager())
            {
                if (!string.IsNullOrEmpty(siteName))
                {
                    result = serverManager.Sites.Where(s =>
                        s.Name.Equals(siteName, StringComparison.InvariantCultureIgnoreCase) ||
                        s.Bindings.Any(b => b.Host.Equals(siteName, StringComparison.InvariantCultureIgnoreCase))
                        );
                }
                else
                {
                    result = serverManager.Sites;
                }
                result = result?.Where(s =>
                    s.GetAppPoolName() != null &&
                    serverManager.ApplicationPools[s.GetAppPoolName()].ManagedRuntimeVersion
                    .Equals(managedVersion, StringComparison.InvariantCultureIgnoreCase));

                if (!result.Any())
                    return null;

                result = result.ToList();
            }

            return result.ToList();
        }

        internal static void UpdateLocalIis(string siteName = "", bool removeInstalled = false)
        {
            var sites = GetSitesByName(siteName);
            if (!sites.Any())
                return;

            var ty = removeInstalled ? null : typeof(AspNetInsight4.ResponseTracker);
            var mName = _httpModuleName;
            foreach (var site in sites)
            {
                InstallModule(site.Name, mName, ty, true, removeInstalled);
            }
        }
        
        public static bool HasValue(this string[] values)
        {
            if (values == null)
                return false;
            if (values.Length > 0)
                return false;

            return values.TrimAll() != null;
        }

        public static string[] TrimAll(this string[] values)
        {
            if (values == null)
                return null;

            var lst = values.ToList();
            lst.RemoveAll(s => string.IsNullOrWhiteSpace(s));

            if (!lst.Any())
                return null;

            return lst.ConvertAll(s => s.Trim()).ToArray();
        }
    }
}
