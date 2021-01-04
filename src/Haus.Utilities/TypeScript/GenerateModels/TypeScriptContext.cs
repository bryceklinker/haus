using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Haus.Utilities.TypeScript.GenerateModels
{
    public interface ITypeScriptGeneratorContext
    {
        bool IsMissingModelForType(Type type);
        TypeScriptModel GetModelForType(Type type);
        void Add(TypeScriptModel model);
        TypescriptBarrelModel GetBarrel();
    }
    
    public class TypeScriptGeneratorContext : ITypeScriptGeneratorContext
    {
        private readonly Dictionary<Type, TypeScriptModel> _models = new();

        public TypeScriptModel[] GetAll() => _models.Values.ToArray();

        public bool IsMissingModelForType(Type type)
        {
            return !_models.ContainsKey(type);
        }

        public TypeScriptModel GetModelForType(Type type)
        {
            return IsMissingModelForType(type) ? null : _models[type];
        }

        public void Add(TypeScriptModel model)
        {
            _models.TryAdd(model.ModelType, model);
        }

        public TypescriptBarrelModel GetBarrel()
        {
            var builder = new StringBuilder();
            foreach (var model in GetAll())
                builder.AppendLine($"export * from './{Path.GetFileNameWithoutExtension(model.FileName)}'");
            return new TypescriptBarrelModel("index.ts", builder.ToString());
        }
    }
}