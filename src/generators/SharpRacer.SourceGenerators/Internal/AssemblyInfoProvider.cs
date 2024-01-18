using System.Reflection;

namespace SharpRacer.SourceGenerators.Internal;
internal static class AssemblyInfoProvider
{
    internal static Assembly GetAssembly()
    {
        return typeof(AssemblyInfoProvider).Assembly;
    }

    internal static string GetFileVersion()
    {
        return GetAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
    }

    internal static string GetInformationalVersion()
    {
        return GetAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }

    internal static string GetProduct()
    {
        return GetAssembly().GetCustomAttribute<AssemblyProductAttribute>().Product;
    }

    internal static Version GetVersion()
    {
        var assembly = GetAssembly();

        return assembly.GetName().Version;
    }
}
