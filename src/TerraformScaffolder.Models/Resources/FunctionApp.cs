namespace TerraformScaffolder.Models.Resources;

[TerraformModule(
    "github.com/pagopa/dx/tree/main/infra/modules/azure_function_app",
    "Azure Function App - Serverless compute service")]
public class FunctionApp
{
    [TerraformProperty(Required = true)]
    public string Name { get; set; } = null!;

    [TerraformProperty("resource_group_name", Required = true)]
    public string ResourceGroupName { get; set; } = null!;

    [TerraformProperty("private_dns_zone_resource_group_name")]
    public string? PrivateDnsZoneResourceGroup { get; set; }

    [TerraformProperty("pep_subnet_id")]
    public string? PepSubnetId { get; set; }

    [TerraformProperty("runtime_version", DefaultValue = "~4")]
    public string RuntimeVersion { get; set; } = "~4";

    [TerraformProperty("always_on", DefaultValue = "true")]
    public bool AlwaysOn { get; set; } = true;

    [TerraformProperty("health_check_path", DefaultValue = "/api/health")]
    public string HealthCheckPath { get; set; } = "/api/health";
}
