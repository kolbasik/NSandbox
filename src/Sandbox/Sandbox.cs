using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace kolbasik.NSandbox
{
    /// <summary>
    ///     <example>https://msdn.microsoft.com/en-us/library/bb763046(v=vs.110).aspx</example>
    /// </summary>
    public sealed class Sandbox : IDisposable
    {
        private readonly AppDomain sandboxDomain;

        public Sandbox(SandboxConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            sandboxDomain = AppDomain.CreateDomain(
                "Sandbox_" + DateTime.UtcNow.Ticks,
                config.Evidence,
                config.Setup,
                config.PermissionSet,
                config.FullTrustAssemblies.ToArray());
        }

        public void Dispose() => AppDomain.Unload(sandboxDomain);

        public object CreateInstance(string assemblyName, string typeName, params object[] arguments)
        {
            try
            {
                return sandboxDomain.CreateInstanceAndUnwrap(assemblyName, typeName, true,
                    BindingFlags.CreateInstance, null, arguments, CultureInfo.InvariantCulture, null);
            }
            catch (Exception ex)
            {
                var targetInvocationException = ex as TargetInvocationException;
                if (targetInvocationException != null)
                {
                    ex = targetInvocationException.InnerException;
                }
                Trace.TraceError($"Sandbox caught: {ex}");
                ExceptionDispatchInfo.Capture(ex).Throw();
                throw;
            }
        }
    }
}