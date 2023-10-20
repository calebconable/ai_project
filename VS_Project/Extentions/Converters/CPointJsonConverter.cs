using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VS_Project.Model;

namespace VS_Project.Extentions.Converters
{

    public class CPointJsonConverter : JsonConverter<CPoint>
    {
        public override CPoint ReadJson(JsonReader reader, Type objectType, CPoint existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Deserialize the JSON object to a dictionary
            var json = serializer.Deserialize<Dictionary<string, object>>(reader);

            // Call the FromJson method to create a Sample object
            return (CPoint)Sample.FromJson(json);
        }

        public override void WriteJson(JsonWriter writer, CPoint value, JsonSerializer serializer)
        {
            // Use the ToJson method to get the dictionary representation
            var json = value.ToJson();

            // Serialize the dictionary to JSON
            var jObject = JObject.FromObject(json);
            jObject.WriteTo(writer);
        }
    }

}
