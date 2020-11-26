using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace Haus.Core.Common
{
    public static class DefaultMapperFactory
    {
        private static readonly IMapper Mapper;
        
        static DefaultMapperFactory()
        {
            Mapper = new Mapper(new MapperConfiguration(config =>
            {
                config.AddProfiles(DiscoverProfiles());
            }));
        }
        
        public static IMapper GetMapper()
        {
            return Mapper;
        }

        private static IEnumerable<Profile> DiscoverProfiles()
        {
            return typeof(DefaultMapperFactory).Assembly
                .GetExportedTypes()
                .Where(t => t.BaseType == typeof(Profile))
                .Select(Activator.CreateInstance)
                .OfType<Profile>();
        }
    }
}