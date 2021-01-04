using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Utilities.Common.Cli;
using MediatR;

namespace Haus.Utilities.TypeScript.GenerateModels
{
    [Command("typescript", "generate-models")]
    public record GenerateTypeScriptModelsCommand : IRequest
    {
        
    }
    
    public class GenerateTypeScriptModelsCommandHandler : AsyncRequestHandler<GenerateTypeScriptModelsCommand>
    {
        private static readonly string ModelsDirectory = Path.Combine(
            Directory.GetCurrentDirectory(), "..", "Haus.Web.Host", "client-app", "src", "app", "shared", "models", "generated");
        
        private readonly ITypeScriptModelGenerator _generator;

        public GenerateTypeScriptModelsCommandHandler(ITypeScriptModelGenerator generator)
        {
            _generator = generator;
        }

        protected override Task Handle(GenerateTypeScriptModelsCommand request, CancellationToken cancellationToken)
        {
            var context = new TypeScriptGeneratorContext();
            var types = GetAllTypesInCoreModels();
            foreach (var type in types) 
                _generator.Generate(type, context);

            WriteAllModelsToModelsDirectory(context);
            return Task.CompletedTask;
        }

        private static void WriteAllModelsToModelsDirectory(TypeScriptGeneratorContext context)
        {
            if (!Directory.Exists(ModelsDirectory)) Directory.CreateDirectory(ModelsDirectory);
            
            var models = context.GetAll();
            var barrel = context.GetBarrel();
            foreach (var model in models)
                File.WriteAllText(Path.Combine(ModelsDirectory, model.FileName), model.Contents);

            File.WriteAllText(Path.Combine(ModelsDirectory, barrel.FileName), barrel.Contents);
        }

        private static IEnumerable<Type> GetAllTypesInCoreModels()
        {
            return Assembly.GetAssembly(typeof(HausJsonSerializer))
                .GetExportedTypes();
        }
    }
}