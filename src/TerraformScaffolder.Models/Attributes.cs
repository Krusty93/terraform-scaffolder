namespace TerraformScaffolder.Models;

[AttributeUsage(AttributeTargets.Class)]
public class TerraformModuleAttribute(string modulePath, string description, string shortName) : Attribute
{
    public string ModulePath { get; } = modulePath;

    public string Description { get; } = description;

    public string ShortName { get; } = shortName;

}

[AttributeUsage(AttributeTargets.Property)]
public class TerraformPropertyAttribute(string? name = null) : Attribute
{
    public string? Name { get; } = name;

    public bool Required { get; set; }

    public string? DefaultValue { get; set; }
}
