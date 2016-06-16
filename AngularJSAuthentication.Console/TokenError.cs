using Newtonsoft.Json;

namespace AngularJSAuthentication.ConsoleApp
{
    public class TokenError
    {
        [JsonProperty("error_description")]
        public string Message { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }
}