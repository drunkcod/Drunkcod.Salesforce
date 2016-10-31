using Newtonsoft.Json;

namespace Drunkcod.Salesforce
{
	public class SalesforceTokenResponse
	{
		[JsonProperty("token_type")] public string TokenType;
		[JsonProperty("access_token")] public string AccessToken;
		[JsonProperty("instance_url")] public string InstanceUrl;
	}
}
