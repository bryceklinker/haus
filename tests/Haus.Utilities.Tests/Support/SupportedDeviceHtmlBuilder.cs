namespace Haus.Utilities.Tests.Support
{
    public class SupportedDeviceHtmlBuilder
    {
        private const string Html = @"<tr>
                            <td><a>{{model}}</a></td>
                            <td>description</td>
                            <td><img src=""some.jpg""></td>
                         </tr>";
        
        private string _model;

        public SupportedDeviceHtmlBuilder WithModel(string model)
        {
            _model = model;
            return this;
        }

        public string Build()
        {
            try
            {
                return Html.Replace("{{model}}", _model);
            }
            finally
            {
                _model = null;
            }
        }
    }
}