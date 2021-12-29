using System;
using System.Collections.Generic;

namespace Haus.Utilities.Tests.Support;

public class SupportedDevicesPageHtmlBuilder
{
    private const string Html = @"
        <html>
            <head></head>
            <body>
                <header></header>
                <main>
                    <h1></h1>
                    <style></style>
                    <p></p>
                    <p></p>
                    {{vendors}}
                </main>
            </body>
        </html>
        ";

    private List<string> _vendors = new();

    public SupportedDevicesPageHtmlBuilder WithVendor(Action<SupportedVendorHtmlBuilder> configure)
    {
        var builder = new SupportedVendorHtmlBuilder();
        configure(builder);
        _vendors.Add(builder.Build());
        return this;
    }

    public string Build()
    {
        try
        {
            return Html.Replace("{{vendors}}", string.Join(" ", _vendors));
        }
        finally
        {
            _vendors = new List<string>();
        }
    }
}