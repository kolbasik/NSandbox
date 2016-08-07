using System;
using System.Linq;
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
            Assert.NotNull(sandbox.Evidence);
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

            Assert.Equal(current.Evidence, sandbox.Evidence);
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
            var current = AppDomain.CurrentDomain;

            // act
            var sandbox = SandboxConfig.FromCurrentDomain().UseBinPath(@".\PLUGINS");

            // assert
            Assert.NotNull(sandbox);
            Assert.Matches("PLUGINS$", sandbox.Setup.PrivateBinPath);
            Assert.Equal(sandbox.Setup.PrivateBinPath, sandbox.Setup.ShadowCopyDirectories);
        }

        [Fact]
        public void UseBinPath_should_use_an_absolute_path_as_is()
        {
            // arrange
            var current = AppDomain.CurrentDomain;

            // act
            var sandbox = SandboxConfig.FromCurrentDomain().UseBinPath(@"C:\TEMP\");

            // assert
            Assert.NotNull(sandbox);
            Assert.Equal(@"C:\TEMP\", sandbox.Setup.PrivateBinPath);
            Assert.Equal(sandbox.Setup.PrivateBinPath, sandbox.Setup.ShadowCopyDirectories);
        }
    }
}