using Spectre.Console;
using TerraformScaffolder.Generated;

namespace TerraformScaffolder.Cli;

public class Wizard
{
    public static async Task<(string ModuleType, Dictionary<string, object> Properties)> RunInteractiveWizardAsync()
    {
        var modules = TerraformModuleFactory.GetAvailableModules();

        // Create selection prompt for modules
        string moduleType = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select the desired [green]module[/]:")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more modules)[/]")
                .AddChoices(modules.Keys)
                .UseConverter(key => $"{key} - {modules[key].Description}"));

        // Collect properties through interactive prompts
        var properties = new Dictionary<string, object>();

        await AnsiConsole.Status()
            .StartAsync("Preparing module configuration...", async ctx =>
            {
                await Task.Delay(1);

                // Always prompt for name first
                var name = AnsiConsole.Prompt(
                    new TextPrompt<string>("Enter the [green]resource name[/]:")
                        .Validate(name =>
                        {
                            if (string.IsNullOrWhiteSpace(name))
                                return ValidationResult.Error("Name cannot be empty");
                            if (name.Length > 24)
                                return ValidationResult.Error("Name must be 24 characters or less");
                            if (!name.All(c => char.IsLetterOrDigit(c) || c == '-'))
                                return ValidationResult.Error("Name can only contain letters, numbers, and hyphens");
                            return ValidationResult.Success();
                        }));

                properties["name"] = name;

                // Resource group is also commonly required
                var resourceGroup = AnsiConsole.Prompt(
                    new TextPrompt<string>("Enter the [green]resource group name[/]:")
                        .Validate(rg =>
                        {
                            if (string.IsNullOrWhiteSpace(rg))
                                return ValidationResult.Error("Resource group cannot be empty");
                            return ValidationResult.Success();
                        }));

                properties["resource_group_name"] = resourceGroup;

                // Add module-specific optional properties
                foreach (var property in GetOptionalPropertiesForModule(moduleType))
                {
                    if (AnsiConsole.Confirm($"Do you want to configure {property}?", false))
                    {
                        var value = AnsiConsole.Prompt(
                            new TextPrompt<string>($"Enter value for [green]{property}[/]:"));
                        properties[property] = value;
                    }
                }
            });

        return (moduleType, properties);
    }

    private static IEnumerable<string> GetOptionalPropertiesForModule(string moduleType)
    {
        // This could be enhanced to read from attributes/metadata
        // For now, returning common optional properties
        return moduleType switch
        {
            "functionapp" => new[]
            {
                "runtime_version",
                "always_on",
                "health_check_path",
                "private_dns_zone_resource_group_name",
                "pep_subnet_id"
            },
            "appservice" => new[]
            {
                "sku_name",
                "allowed_ips",
                "app_settings"
            },
            _ => Array.Empty<string>()
        };
    }
}
