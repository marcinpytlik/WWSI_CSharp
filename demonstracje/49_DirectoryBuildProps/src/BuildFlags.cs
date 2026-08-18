using System.Reflection;

namespace Demo49;

public static class BuildFlags
{
#if DEMO49
    public const bool FromNestedBuildProps = true;
#else
    public const bool FromNestedBuildProps = false;
#endif

    public static string? AssemblyTitle()
        => typeof(BuildFlags).Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
}

public static class Program
{
    public static int Main()
    {
        Console.WriteLine($"DEMO49={FromNestedBuildProps} title={AssemblyTitle()}");
        return 0;
    }
}
