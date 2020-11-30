using System;
using System.Collections.Generic;

namespace Haus.Utilities.Tests.Support
{
    public class SupportedVendorHtmlBuilder
    {
        private const string Html = @"
        <h3>
            {{name}}
            <a href=""#{{name}}"">#</a>
        </h3>
        <table>
            <thead>
                <tr>
                    <td>Model</td>
                    <td>Description</td>
                    <td>Image</td>
                </tr>
            </thead>
            <tbody>
                {{devices}}
            </tbody>
        </table>
        ";

        private string _name;
        private List<string> _devices = new List<string>();

        public SupportedVendorHtmlBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public SupportedVendorHtmlBuilder WithDevice(Action<SupportedDeviceHtmlBuilder> configure)
        {
            var builder = new SupportedDeviceHtmlBuilder();
            configure(builder);
            _devices.Add(builder.Build());
            return this;
        }

        public string Build()
        {
            try
            {
                return Html.Replace("{{name}}", _name)
                    .Replace("{{devices}}", string.Join(" ", _devices));
            }
            finally
            {
                _name = null;
                _devices = new List<string>();
            }
        }
    }
}