using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Cqrs.Commands;
using Haus.Utilities.Common.Cli;
using Haus.Utilities.TypeScript.GenerateModels;

namespace Haus.Utilities.TypeScript.Commands;

[Command("typescript", "generate-models")]
public record GenerateTypeScriptModelsCommand : ICommand;

public class GenerateTypeScriptModelsCommandHandler(ITypeScriptModelGenerator generator)
    : ICommandHandler<GenerateTypeScriptModelsCommand>
{
    private static readonly string ModelsDirectory = Path.Combine(
        Directory.GetCurrentDirectory(),
        "..",
        "Haus.Web.Host",
        "client-app",
        "src",
        "app",
        "shared",
        "models",
        "generated"
    );

    public Task Handle(GenerateTypeScriptModelsCommand request, CancellationToken cancellationToken)
    {
        var context = new TypeScriptGeneratorContext();
        var types = GetAllTypesInCoreModels();
        foreach (var type in types)
            generator.Generate(type, context);

        WriteAllModelsToModelsDirectory(context);
        return Task.CompletedTask;
    }

    private static void WriteAllModelsToModelsDirectory(TypeScriptGeneratorContext context)
    {
        if (!Directory.Exists(ModelsDirectory))
        {
            Directory.Delete(ModelsDirectory, true);
            Directory.CreateDirectory(ModelsDirectory);
        }

        var models = context.GetAll();
        var barrel = context.GetBarrel();
        foreach (var model in models)
            File.WriteAllText(Path.Combine(ModelsDirectory, model.FileName), model.Contents);

        File.WriteAllText(Path.Combine(ModelsDirectory, barrel.FileName), barrel.Contents);
    }

    private static IEnumerable<Type> GetAllTypesInCoreModels()
    {
        var assembly = Assembly.GetAssembly(typeof(HausJsonSerializer));
        ArgumentNullException.ThrowIfNull(assembly);
        return assembly.GetExportedTypes();
    }
}
