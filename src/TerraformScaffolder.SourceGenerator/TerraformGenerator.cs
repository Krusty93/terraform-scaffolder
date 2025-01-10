using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TerraformScaffolder.Models;

namespace TerraformScaffolder.SourceGenerator;

[Generator]
public class TerraformGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        GeneratorLogging.SetLogFilePath("F:\\terraform-scaffolder\\log.txt");

        IncrementalValuesProvider<ClassDeclarationSyntax> terraformClassesProvider = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: (SyntaxNode node, CancellationToken token) =>
            {
                return node is ClassDeclarationSyntax classDeclarationSyntax &&
                       classDeclarationSyntax.AttributeLists.Any();
            },
            transform: (GeneratorSyntaxContext ctx, CancellationToken token) =>
            {
                return (ClassDeclarationSyntax)ctx.Node;
            });

        context.RegisterSourceOutput(
            terraformClassesProvider,
            (sourceProductionContext, terraformClass) =>
            {
                try
                {
                    Execute(terraformClass, sourceProductionContext);
                }
                catch (Exception ex)
                {
                    GeneratorLogging.LogMessage($"[-] Exception occurred in generator: {ex}", LoggingLevel.Error);
                }
                finally
                {
                    GeneratorLogging.EndLogging();
                }
            });
    }

    private static void Execute(ClassDeclarationSyntax terraformClass, SourceProductionContext context)
    {
        GeneratorLogging.LogMessage($"[+] Processing Terraform class: {terraformClass.Identifier}");

        var source = new StringBuilder();

        //var classMembers = terraformClass.Members;

        //var getAvailableModulesMethod = classMembers.FirstOrDefault(x => x is MethodDeclarationSyntax mds /*&& mds.Identifier.Text == "GetAvailableModules"*/);

        //GeneratorLogging.LogMessage($"[+] method found: {getAvailableModulesMethod}");

        var attribute = terraformClass.AttributeLists
            .SelectMany(al => al.Attributes)
            .FirstOrDefault(attr => attr.Name.ToString() == "TerraformModule");

        GeneratorLogging.LogMessage($"[+] attribute: {attribute}");

        // TODO: improve
        var modulePath = attribute?.ArgumentList?.Arguments.FirstOrDefault()?.ToString();
        var moduleDescription = attribute?.ArgumentList?.Arguments.Skip(1).FirstOrDefault()?.ToString();
        var moduleShortName = attribute?.ArgumentList?.Arguments.Skip(2).FirstOrDefault()?.ToString();

        source.AppendLine($@"
            module ""{moduleShortName}_{{instance_number}}"" {{
                source = ""{modulePath}?ref={{version}}""

                environment = {{
                    prefix          = {{prefix}}
                    env_short       = {{env_short}}
                    location        = {{location}}
                    domain          = {{domain}}
                    app_name        = {{app_name}}
                    instance_number = {{instance_number}}
                }}
            }}");

        builder.AppendLine($@""module """"{{name}}-{moduleClass.Name.ToLowerInvariant()}"""""" {
            {
                ");
        builder.AppendLine($@""  source = """"{modulePath}""""");

                //GeneratorLogging.LogMessage($"[+] attribute path: {modulePath}");
                //GeneratorLogging.LogMessage($"[+] attribute description: {moduleDescription}");

                //var usings = terraformClass.SyntaxTree.GetCompilationUnitRoot().Usings;

                //GeneratorLogging.LogMessage($"[+] usings: {usings}");

                //foreach (var usingStatement in usings)
                //{
                //    source.AppendLine(usingStatement.ToString());
                //}

                //source.AppendLine();

                //SyntaxNode classNamespace = terraformClass.Parent;

                //while (classNamespace is not NamespaceDeclarationSyntax)
                //{
                //    classNamespace = classNamespace.Parent;
                //}

                //source.AppendLine($"namespace {((NamespaceDeclarationSyntax)classNamespace).Name};");

                //source.AppendLine($"public {terraformClass.Modifiers} class {terraformClass.Identifier}");
                //source.AppendLine("{");

                //if (attributeValue != null)
                //{
                //    source.AppendLine($@"            [""{terraformClass.Identifier.Text.ToLowerInvariant()}""] = (""{attributeValue}"", ""Description""),");
                //}

                source.AppendLine(@"
                };
            }
        }");

                //        source.AppendLine(@"
                //using System;
                //using System.Text;

                //namespace TerraformScaffolder.Generated;

                ///// <summary>
                ///// Produce Terraform code for the specified module type
                ///// </summary>
                //public static class TerraformModuleFactory
                //{
                //    /// <summary>
                //    /// Get list of available Terraform modules
                //    /// </summary>
                //    public static Dictionary<string, (string ModulePath, string Description)> GetAvailableModules()
                //    {
                //        return new Dictionary<string, (string, string)>
                //        {}
                //    }");
                //        source.AppendLine(@"
                //}"
                //        );

                var output = source.ToString();

                GeneratorLogging.LogMessage($"[+] generated code: {output}");

                context.AddSource("TerraformModuleFactory.Generated.cs", output);
            }

    //public void Execute(GeneratorExecutionContext context)
    //{
    //    if (context.SyntaxReceiver is not SyntaxReceiver receiver)
    //        return;

            //    var terraformModuleAttribute = context.Compilation.GetTypeByMetadataName("TerraformScaffolder.Models.TerraformModuleAttribute");
            //    var terraformPropertyAttribute = context.Compilation.GetTypeByMetadataName("TerraformScaffolder.Models.TerraformPropertyAttribute");

            //    if (terraformModuleAttribute == null || terraformPropertyAttribute == null)
            //        return;

            //    var moduleClasses = new List<INamedTypeSymbol>();
            //    foreach (var candidateClass in receiver.CandidateClasses)
            //    {
            //        var model = context.Compilation.GetSemanticModel(candidateClass.SyntaxTree);
            //        if (model.GetDeclaredSymbol(candidateClass) is INamedTypeSymbol classSymbol)
            //        {
            //            var moduleAttr = classSymbol.GetAttributes()
            //                .FirstOrDefault(ad => ad.AttributeClass?.Equals(terraformModuleAttribute, SymbolEqualityComparer.Default) ?? false);

            //            if (moduleAttr != null)
            //                moduleClasses.Add(classSymbol);
            //        }
            //    }

            //    GenerateModuleFactory(context, moduleClasses);
            //}

    private static void GenerateModuleFactory(GeneratorExecutionContext context, List<INamedTypeSymbol> moduleClasses)
    {
        var source = new StringBuilder();
        source.AppendLine(@"using System;
using System.Text;

namespace TerraformScaffolder.Generated
{
    /// <summary>
    /// Produce Terraform code for the specified module type
    /// </summary>
    public static class TerraformModuleFactory
    {
        /// <summary>
        /// Get list of available Terraform modules
        /// </summary>
        public static Dictionary<string, (string ModulePath, string Description)> GetAvailableModules()
        {
            return new Dictionary<string, (string, string)>
            {");

        foreach (var moduleClass in moduleClasses)
        {
            var moduleAttr = moduleClass.GetAttributes()
                .First(ad => ad.AttributeClass?.Name == "TerraformModuleAttribute");

            var modulePath = moduleAttr.ConstructorArguments[0].Value?.ToString();
            var description = moduleAttr.ConstructorArguments[1].Value?.ToString();
            var moduleType = moduleClass.Name.ToLowerInvariant();

            source.AppendLine($@"                [""{moduleType}""] = (""{modulePath}"", ""{description}""),");
        }

        source.AppendLine(@"            };
        }

        /// <summary>
        /// Generate Terraform code for the specified module type
        /// </summary>
        /// <param name=""moduleType"">Module name</param>
        /// <param name=""properties"">Dictionary of properties</param>
        public static string GenerateTerraformCode(string moduleType, Dictionary<string, object> properties)
        {
            return moduleType switch
            {");

        foreach (var moduleClass in moduleClasses)
        {
            var moduleType = moduleClass.Name.ToLowerInvariant();
            var moduleAttr = moduleClass.GetAttributes()
                .First(ad => ad.AttributeClass?.Name == "TerraformModuleAttribute");
            var modulePath = moduleAttr.ConstructorArguments[0].Value?.ToString();

            source.AppendLine($@"                ""{moduleType}"" => Generate{moduleClass.Name}(properties),");
        }

        source.AppendLine(@"                _ => throw new ArgumentException($""Unknown module type: {moduleType}"")
            };
        }");

        // Generate individual module methods
        foreach (var moduleClass in moduleClasses)
        {
            GenerateModuleMethod(source, moduleClass);
        }

        source.AppendLine(@"    }
}");

        //context.AddSource("TerraformModuleFactory.g.cs", source.ToString());
    }

    private static void GenerateModuleMethod(StringBuilder source, INamedTypeSymbol moduleClass)
    {
        var moduleAttr = moduleClass.GetAttributes()
            .First(ad => ad.AttributeClass?.Name == "TerraformModuleAttribute");
        var modulePath = moduleAttr.ConstructorArguments[0].Value?.ToString();

        source.AppendLine($@"
        private static string Generate{moduleClass.Name}(Dictionary<string, object> properties)
        {{
            var builder = new StringBuilder();
            
            // Validate required properties
            var missingProps = new List<string>();");

        foreach (var property in moduleClass.GetMembers().OfType<IPropertySymbol>())
        {
            var propAttr = property.GetAttributes()
                .FirstOrDefault(ad => ad.AttributeClass?.Name == "TerraformPropertyAttribute");

            if (propAttr != null)
            {
                var required = propAttr.NamedArguments
                    .FirstOrDefault(kvp => kvp.Key == "Required").Value.Value as bool? ?? false;

                if (required)
                {
                    var propName = propAttr.ConstructorArguments.Length > 0 &&
                                 propAttr.ConstructorArguments[0].Value != null
                        ? propAttr.ConstructorArguments[0].Value.ToString()
                        : property.Name.ToLowerInvariant();

                    source.AppendLine($@"
            if (!properties.ContainsKey(""{propName}""))
                missingProps.Add(""{propName}"");");
                }
            }
        }

        source.AppendLine(@"
            if (missingProps.Count > 0)
                throw new ArgumentException($""Missing required properties: {string.Join(', ', missingProps)}"");");

        // Generate module block
        // source.AppendLine($@"
        //     var name = properties[""name""].ToString();
        //     builder.AppendLine($@""module """"{{name}}-{moduleClass.Name.ToLowerInvariant()}"""""" {{");
        //     builder.AppendLine($@""  source = """"{modulePath}""""");

        foreach (var property in moduleClass.GetMembers().OfType<IPropertySymbol>())
        {
            var propAttr = property.GetAttributes()
                .FirstOrDefault(ad => ad.AttributeClass?.Name == "TerraformPropertyAttribute");

            if (propAttr != null)
            {
                var propName = propAttr.ConstructorArguments.Length > 0 &&
                             propAttr.ConstructorArguments[0].Value != null
                    ? propAttr.ConstructorArguments[0].Value.ToString()
                    : property.Name.ToLowerInvariant();

                var defaultValue = propAttr.NamedArguments
                    .FirstOrDefault(kvp => kvp.Key == "DefaultValue").Value.Value?.ToString();

                if (defaultValue != null)
                {
                    source.AppendLine($@"
            if (properties.TryGetValue(""{propName}"", out var {propName}Value))
                builder.AppendLine($@""  {propName} = """"{{Convert.ToString({propName}Value)}}"""""");
            else
                builder.AppendLine($@""  {propName} = """"{defaultValue}"""""");");
                }
                else
                {
                    source.AppendLine($@"
            if (properties.TryGetValue(""{propName}"", out var {propName}Value))
                builder.AppendLine($@""  {propName} = """"{{Convert.ToString({propName}Value)}}"""""");");
                }
            }
        }

        source.AppendLine(@"
            builder.AppendLine(""}"");
            return builder.ToString();
        }");
    }
}
