namespace Craft.CraftModule.Attributes;

using System;

/// <summary>
///  Represents an attribute that specifies dependencies for a Craft module.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute : Attribute
{
    public Type[] Dependencies { get; }

    public DependsOnAttribute(params Type[] dependencies)
    {
        Dependencies = dependencies;
    }
}
