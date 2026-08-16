using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Haus.Web.Host.Common.Mvc;

public class ExcludeControllerFeatureProvider(Type excludedControllerType) : ControllerFeatureProvider
{
    private readonly TypeInfo _excludedTypeInfo = excludedControllerType.GetTypeInfo();

    protected override bool IsController(TypeInfo typeInfo)
    {
        return typeInfo != _excludedTypeInfo && base.IsController(typeInfo);
    }

    public static void ReplaceDefaultProvider(ApplicationPartManager manager, Type excludedControllerType)
    {
        var defaultProvider = manager.FeatureProviders.OfType<ControllerFeatureProvider>().ToList();
        foreach (var provider in defaultProvider)
            manager.FeatureProviders.Remove(provider);

        manager.FeatureProviders.Add(new ExcludeControllerFeatureProvider(excludedControllerType));
    }
}
