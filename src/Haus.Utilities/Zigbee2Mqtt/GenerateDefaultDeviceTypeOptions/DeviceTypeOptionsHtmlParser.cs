using System.Collections.Generic;
using System.Linq;
using Haus.Zigbee.Host.Configuration;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace Haus.Utilities.Zigbee2Mqtt.GenerateDefaultDeviceTypeOptions
{
    public interface IDeviceTypeOptionsHtmlParser
    {
        IEnumerable<DeviceTypeOptions> Parse(string html);
    }

    public class DeviceTypeOptionsHtmlParser : IDeviceTypeOptionsHtmlParser
    {
        private readonly ILogger<DeviceTypeOptionsHtmlParser> _logger;

        public DeviceTypeOptionsHtmlParser(ILogger<DeviceTypeOptionsHtmlParser> logger)
        {
            _logger = logger;
        }

        public IEnumerable<DeviceTypeOptions> Parse(string html)
        {
            return LocateVendorNodesInHtml(html)
                .SelectMany(LocateDevicesForVendor);
        }

        private static IEnumerable<DeviceTypeOptions> LocateDevicesForVendor(HtmlNode vendor)
        {
            var vendorName = ScrubText(vendor.InnerText);
            var tableBody = LocateTableBody(vendor);
            return tableBody.Descendants("tr")
                .Select(row => CreateDeviceTypeOptions(row, vendorName));
        }

        private static DeviceTypeOptions CreateDeviceTypeOptions(HtmlNode row, string vendor)
        {
            var modelColumn = row.Descendants("td").First();
            return new DeviceTypeOptions
            {
                Model = modelColumn.InnerText,
                Vendor = vendor
            };
        }
        
        private static IEnumerable<HtmlNode> LocateVendorNodesInHtml(string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var body = document.DocumentNode.SelectNodes("//body");
            return body.Descendants("h3");
        }
        
        private static HtmlNode LocateTableBody(HtmlNode node)
        {
            var nextTag = node.NextSibling;
            while (nextTag.Name != "table")
            {
                nextTag = nextTag.NextSibling;
            }
            return nextTag.Descendants("tbody").First();   
        }

        private static string ScrubText(string text)
        {
            return text
                .Replace("#", "")
                .Trim();
        }
    }
}