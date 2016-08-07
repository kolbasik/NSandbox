using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Policy;

namespace kolbasik.NSandbox
{
    /// <summary>
    ///     <example>https://msdn.microsoft.com/en-us/library/bb763046(v=vs.110).aspx</example>
    /// </summary>
    /// <seealso cref="System.MarshalByRefObject" />
    [Serializable]
    public sealed class Sandbox : MarshalByRefObject, IDisposable
    {
        private readonly AppDomain sandboxDomain;
        private readonly AppDomainSetup sandboxDomainSetup;

        public Sandbox(
            Evidence domainEvidence = null,
            AppDomainSetup domainSetup = null,
            PermissionSet domainPermission = null,
            params StrongName[] fullTrustAssemblies)
        {
            var currentDomain = AppDomain.CurrentDomain;
            sandboxDomainSetup = domainSetup ?? currentDomain.SetupInformation;
            sandboxDomain = AppDomain.CreateDomain(
                "Sandbox_" + DateTime.UtcNow.Ticks,
                domainEvidence ?? currentDomain.Evidence,
                sandboxDomainSetup,
                domainPermission ?? currentDomain.PermissionSet,
                fullTrustAssemblies);
        }

        public void Dispose() => AppDomain.Unload(sandboxDomain);

        public T CreateInstance<T>(string assemblyName, string typeName)
        {
            try
            {
                var instance = (T)sandboxDomain.CreateInstanceAndUnwrap(assemblyName, typeName);
                return instance;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Sandbox caught: {ex}");
                throw;
            }
        }

        public static AppDomainSetup CreateSandboxSetup(string path, AppDomainSetup domainSetup = null)
        {
            var sendboxSetup = ShallowCopy(domainSetup ?? AppDomain.CurrentDomain.SetupInformation);
            var binPath = Path.IsPathRooted(path) ? path : string.Concat(sendboxSetup.PrivateBinPath, @"\", path);
            sendboxSetup.ApplicationName = "Sandbox_" + DateTime.UtcNow.Ticks;
            sendboxSetup.ShadowCopyDirectories = sendboxSetup.PrivateBinPath = binPath;
            return sendboxSetup;
        }

        public static T ShallowCopy<T>(T source)
        {
            var method = typeof(T).GetMethod(@"MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            return (T)method.Invoke(source, new object[0]);
        }
    }
}
