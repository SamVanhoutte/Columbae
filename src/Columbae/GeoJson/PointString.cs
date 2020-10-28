using Newtonsoft.Json;

namespace Columbae.GeoJson
{
    internal struct Pointstring
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("coordinates")]
        public double[] Coordinates { get; set; }

    }
}