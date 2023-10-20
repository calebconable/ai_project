using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VS_Project.Extentions.Converters;

namespace VS_Project.Singletone
{
    public static class JsonSerializerSignletone
    {
        public static JsonSerializerSettings SETTINGS = new JsonSerializerSettings
        {
            Converters = { new SampleJsonConverter(), new CPointJsonConverter() },
            Formatting = Formatting.Indented
        };
    }
}
