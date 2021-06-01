using Newtonsoft.Json;

namespace Core
{
    public struct ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; private set; }
        [JsonProperty("prefix")]
        public string Prefix { get; private set; }
        [JsonProperty("RappId")]
        public string appId { get; private set; }
        [JsonProperty("Rsecret")]
        public string secret { get; private set; }
        [JsonProperty("Raccess")]
        public string access { get; private set; }
        [JsonProperty("RRefresh")]
        public string Refresh { get; private set; }
        [JsonProperty("BannedWords")]
        public string[] bannedwords { get; private set; }
        [JsonProperty("tenorkey")]
        public string tenorkey { get; private set; }
        [JsonProperty("DBConnection")]
        public string connectionstring { get; private set; }
    }
}
