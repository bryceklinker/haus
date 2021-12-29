using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Haus.Api.Client.Common;

public class QueryParameters : IEnumerable
{
    private readonly List<KeyValuePair<string, string>> _parameters = new();

    public void Add(string key, string value)
    {
        _parameters.Add(new KeyValuePair<string, string>(key, value));
    }

    public override string ToString()
    {
        var parameters = _parameters
            .Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}");
        return string.Join("&", parameters);
    }

    public IEnumerator GetEnumerator()
    {
        return _parameters.GetEnumerator();
    }
}