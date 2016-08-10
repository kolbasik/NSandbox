using System;

namespace kolbasik.NSandbox
{
    public static class SandboxExtensions
    {
        public static T CreateInstance<T>(this Sandbox sandbox, params object[] arguments)
        {
            return (T) sandbox.CreateInstance(typeof (T), arguments);
        }

        public static object CreateInstance(this Sandbox sandbox, Type type, params object[] arguments)
        {
            return sandbox.CreateInstance(type.Assembly.FullName, type.FullName, arguments);
        }
    }
}