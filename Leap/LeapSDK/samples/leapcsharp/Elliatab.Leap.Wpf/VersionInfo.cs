using Newtonsoft.Json;

namespace Elliatab.Leap
{
    public class VersionInfo
    {
        [JsonProperty("version")]
        public string VersionId { get; private set; }

        internal static VersionInfo DeserializeFromJson(string value)
        {
            var version = JsonConvert.DeserializeObject<VersionInfo>(value);

            return version;
        }
    }
}
