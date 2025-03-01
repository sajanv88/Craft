namespace Craft.CraftModule;

internal static class ModuleRegistry
{
    private static readonly HashSet<Type> RegisteredModules = [];

    public static void Register(Type moduleType)
    {
        if (!typeof(CraftModule).IsAssignableFrom(moduleType))
        {
            throw new ArgumentException(
                $"Type {moduleType.Name} is not a valid module."
            );
        }

        RegisteredModules.Add(moduleType);
    }

    public static bool IsRegistered(Type moduleType)
    {
        return RegisteredModules.Contains(moduleType);
    }

    public static IEnumerable<Type> GetRegisteredModules()
    {
        return RegisteredModules;
    }
}
