using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Xunit;

namespace kolbasik.NSandbox.Tests
{
    public sealed class SandboxConfigTests
    {
        [Fact]
        public void Ctor_should_create_an_instance_with_evidence_and_full_trust_assemblies()
        {
            // act
            var sandbox = new SandboxConfig();

            // assert
            Assert.NotNull(sandbox);
            Assert.NotNull(sandbox.Setup);
            Assert.NotNull(sandbox.Evidence);
            Assert.NotNull(sandbox.PermissionSet);
            Assert.NotNull(sandbox.FullTrustAssemblies);
        }

        [Fact]
        public void FromCurrentDomain_should_copy_settings_from_current_domain()
        {
            // arrange
            var current = AppDomain.CurrentDomain;

            // act
            var sandbox = SandboxConfig.FromCurrentDomain();

            // assert
            Assert.NotNull(sandbox);
            Assert.Matches("Sandbox_.+", sandbox.Setup.ApplicationName);

            Assert.NotSame(current.Evidence, sandbox.Evidence);
            Assert.NotSame(current.SetupInformation, sandbox.Setup);
            Assert.NotSame(current.PermissionSet, sandbox.PermissionSet);

            Assert.Equal(current.Evidence, sandbox.Evidence, EvidenceComparer.Instance);
            Assert.Equal(current.SetupInformation.ConfigurationFile, sandbox.Setup.ConfigurationFile);
            Assert.Equal(current.PermissionSet, sandbox.PermissionSet);
            Assert.Equal(
                current.ApplicationTrust.FullTrustAssemblies.AsEnumerable(),
                sandbox.FullTrustAssemblies.AsEnumerable());
        }

        [Fact]
        public void UseBinPath_should_combine_a_relative_path()
        {
            // arrange
            var config = SandboxConfig.FromCurrentDomain();

            // act
            config.UseBinPath(@".\PLUGINS");

            // assert
            Assert.NotNull(config);
            Assert.Matches("PLUGINS$", config.Setup.PrivateBinPath);
            Assert.Equal(config.Setup.PrivateBinPath, config.Setup.ShadowCopyDirectories);
        }

        [Fact]
        public void UseBinPath_should_use_an_absolute_path_as_is()
        {
            // arrange
            var config = SandboxConfig.FromCurrentDomain();

            // act
            config.UseBinPath(@"C:\TEMP\");

            // assert
            Assert.NotNull(config);
            Assert.Equal(@"C:\TEMP\", config.Setup.PrivateBinPath);
            Assert.Equal(config.Setup.PrivateBinPath, config.Setup.ShadowCopyDirectories);
        }

        private sealed class EvidenceComparer : IEqualityComparer<Evidence>
        {
            public static readonly EvidenceComparer Instance = new EvidenceComparer();

            public bool Equals(Evidence x, Evidence y)
            {
                if (x != null && y != null && x.Count == y.Count)
                {
                    var isHostEvidenceEquals = Equals(x.GetHostEvidence<Zone>(), y.GetHostEvidence<Zone>());
                    var isAssemblyEvidenceEquals = Equals(x.GetAssemblyEvidence<Zone>(), y.GetAssemblyEvidence<Zone>());
                    return isHostEvidenceEquals && isAssemblyEvidenceEquals;
                }
                return false;
            }

            public int GetHashCode(Evidence obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}