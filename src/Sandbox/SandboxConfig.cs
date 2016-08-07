using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace kolbasik.NSandbox
{
    public sealed class SandboxConfig
    {
        public SandboxConfig()
        {
            Evidence = new Evidence();
            PermissionSet = new PermissionSet(PermissionState.None);
            FullTrustAssemblies = new List<StrongName>();
        }

        public Evidence Evidence { get; set; }
        public AppDomainSetup Setup { get; set; }
        public PermissionSet PermissionSet { get; set; }
        public List<StrongName> FullTrustAssemblies { get; }

        public static SandboxConfig FromCurrentDomain()
        {
            var config = new SandboxConfig();
            var currentDomain = AppDomain.CurrentDomain;
            config.Evidence = currentDomain.Evidence;
            config.Setup = currentDomain.SetupInformation;
            config.PermissionSet = currentDomain.PermissionSet;
            config.FullTrustAssemblies.AddRange(currentDomain.ApplicationTrust.FullTrustAssemblies);
            config.Setup.ApplicationName = "Sandbox_" + DateTime.UtcNow.Ticks;
            return config;
        }

        public SandboxConfig UseBinPath(string path)
        {
            var setup = Setup;
            var binPath = Path.IsPathRooted(path) ? path : string.Concat(setup.PrivateBinPath, @"\", path);
            setup.ShadowCopyDirectories = setup.PrivateBinPath = binPath;
            return this;
        }
    }
}