using System;
using System.IO;
using System.Security;
using System.Security.Permissions;
using Xunit;

namespace kolbasik.NSandbox.Tests
{
    public sealed class SandboxTests
    {
        [Fact]
        public void Ctor_should_create_an_instance()
        {
            // arrange
            var config = SandboxConfig.FromCurrentDomain();

            // act
            var sandbox = new Sandbox(config);

            // assert
            Assert.NotNull(sandbox);
        }

        [Fact]
        public void Dispose_should_delete_a_sandbox()
        {
            // arrange
            var config = SandboxConfig.FromCurrentDomain();
            var sandbox = new Sandbox(config);

            // act
            sandbox.Dispose();

            // assert
            Assert.Throws<AppDomainUnloadedException>(() => sandbox.CreateInstance<int>("System", "System.Int32"));
        }

        [Fact]
        public void CreateInstance_should_create_an_instance_by_type_if_trust()
        {
            // arrange
            var config = SandboxConfig.FromCurrentDomain();
            var sandbox = new Sandbox(config);

            var tempFile = Path.GetTempFileName();

            // act
            var type = typeof (StreamWriter);
            var actual = sandbox.CreateInstance<StreamWriter>(type.Assembly.FullName, type.FullName, tempFile);

            // assert
            Assert.NotNull(actual);
            Assert.IsType<StreamWriter>(actual);
        }

        [Fact]
        public void CreateInstance_should_raise_a_security_exception_if_does_not_trust()
        {
            // arrange
            var config = new SandboxConfig();
            config.Setup = new AppDomainSetup {ApplicationBase = @"C:/temp"};
            var sandbox = new Sandbox(config);

            var tempFile = Path.GetTempFileName();

            // act & assert
            var type = typeof (StreamWriter);
            Assert.Throws<SecurityException>(
                () => sandbox.CreateInstance<StreamWriter>(type.Assembly.FullName, type.FullName, tempFile));
        }
    }
}