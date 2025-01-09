namespace TerraformScaffolder.Models;

[AttributeUsage(AttributeTargets.Class)]
public class TerraformModuleAttribute : Attribute
{
    public string ModulePath { get; }
    public string Description { get; }

    public TerraformModuleAttribute(string modulePath, string description)
    {
        ModulePath = modulePath;
        Description = description;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class TerraformPropertyAttribute : Attribute
{
    public string? Name { get; }
    public bool Required { get; set; }
    public string? DefaultValue { get; set; }

    public TerraformPropertyAttribute(string? name = null)
    {
        Name = name;
    }
}
