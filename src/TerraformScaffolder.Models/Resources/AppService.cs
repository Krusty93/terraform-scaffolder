namespace TerraformScaffolder.Models.Resources;

[TerraformModule(
    "github.com/pagopa/dx/tree/main/infra/modules/app_service",
    "Azure App Service - Web application hosting")]
public class AppService
{
    [TerraformProperty(Required = true)]
    public string Name { get; set; } = null!;

    [TerraformProperty("resource_group_name", Required = true)]
    public string ResourceGroupName { get; set; } = null!;

    // Add other properties...
}
