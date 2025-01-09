using Spectre.Console;
using TerraformScaffolder.Cli;
using TerraformScaffolder.Generated;

try
{
    AnsiConsole.Write(
        new FigletText("PagoPA Terraform Scaffolder")
            .Color(Color.Blue));

    (string ModuleType, Dictionary<string, object> Properties) config = await Wizard.RunInteractiveWizardAsync();
    var terraformCode = TerraformModuleFactory.GenerateTerraformCode(
        config.ModuleType,
        config.Properties);

    // Show preview
    AnsiConsole.WriteLine();
    AnsiConsole.Write(new Rule("[yellow]Generated Terraform Code[/]"));
    AnsiConsole.Write(new Panel(terraformCode)
        .Header("Preview")
        .Padding(1, 1)
        .Border(BoxBorder.Rounded));

    var confirmed = AnsiConsole.Prompt(
        new ConfirmationPrompt("Do you want to save this configuration?")
        {
            DefaultValue = true
        });

    if (!confirmed)
        throw new OperationCanceledException("Operation cancelled by user");

    // Save the file
    var fileName = $"{config.Properties["name"]}.tf";
    await File.WriteAllTextAsync(fileName, terraformCode);

    AnsiConsole.MarkupLine($"[green]Successfully saved Terraform configuration to [bold]{fileName}[/][/]");
    return 0;
}
catch (OperationCanceledException)
{
    AnsiConsole.MarkupLine("[yellow]Operation cancelled[/]");
    return 1;
}
catch (Exception ex)
{
    AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
    return 1;
}

