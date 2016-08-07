using Xunit;

namespace kolbasik.NSandbox.Tests
{
    public sealed class SandboxTests
    {
        [Fact]
        public void Test()
        {
            // arrange
            var sandbox = new Sandbox();

            // assert
            Assert.NotNull(sandbox);
        }
    }
}
