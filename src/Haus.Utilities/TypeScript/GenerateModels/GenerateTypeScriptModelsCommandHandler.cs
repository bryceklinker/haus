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
        private readonly ITypeScriptModelGenerator _generator;

        public GenerateTypeScriptModelsCommandHandler(ITypeScriptModelGenerator generator)
        {
            _generator = generator;
        }

        protected override Task Handle(GenerateTypeScriptModelsCommand request, CancellationToken cancellationToken)
        {
            var context = new TypeScriptGeneratorContext();
            var types = Assembly.GetAssembly(typeof(HausJsonSerializer))
                .GetExportedTypes();
            foreach (var type in types) 
                _generator.Generate(type, context);

            var models = context.GetAll();
            var barrel = context.GetBarrel();
            foreach (var model in models)
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), model.FileName), model.Contents);

            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), barrel.FileName), barrel.Contents);
            return Task.CompletedTask;
        }
    }
}