namespace TerraformScaffolder.Cli.Factories;

/// <summary>
/// Produce HCL code for Terraform modules
/// </summary>
public class TerraformModuleFactory
{
    /// <summary>
    /// Get list of available Terraform modules
    /// </summary>
    public static Dictionary<string, string> GetAvailableModules() =>
        new()
        {

        };
}
