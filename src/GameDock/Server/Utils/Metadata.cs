using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace GameDock.Server.Utils;

public static class Metadata
{
    public static T Deserialize<T>(this IDictionary<string, tusdotnet.Models.Metadata> metadata)
    {
        var jObject = new JObject();
        foreach (var data in metadata)
        {
            jObject.Add(data.Key, JToken.Parse(data.Value.GetString(Encoding.Default)));
        }

        return jObject.ToObject<T>();
    }
}