using Newtonsoft.Json;

namespace Drunkcod.Salesforce
{

	public class SalesforceVersionInfo
	{
		[JsonProperty("label")] public string Label;
		[JsonProperty("url")] public string Url;
		[JsonProperty("version")] public string Version;
	}

}
