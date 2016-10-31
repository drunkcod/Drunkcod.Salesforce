using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Drunkcod.Salesforce
{
	public class SalesforceStrongClient
	{
		class SalesforceActions
		{
			[JsonProperty("query")] public string Query;
		}

		readonly SalesforceClient client;
		readonly SalesforceActions actions;

		SalesforceStrongClient(SalesforceClient client, SalesforceActions actions) {
			this.client = client;
			this.actions = actions;
		}

		public async static Task<SalesforceStrongClient> CreateAsync(SalesforceClient sf, string versionUrl) => new SalesforceStrongClient(sf, await sf.GetAsync<SalesforceActions>(versionUrl));
		public async static Task<SalesforceStrongClient> CreateAsync(SalesforceClient sf) => await CreateAsync(sf, (await sf.ListVersionsAsync()).Last().Url);

		public async Task<SalesforceQueryResult<JToken>> QueryAsync(string query) {
			var nextResult = client.GetAsync($"{actions.Query}?q={WebUtility.UrlEncode(query)}");
			var records = Enumerable.Empty<JToken>();
			for(;;) {
				var response = await nextResult;
				var content = JObject.Parse(await response.Content.ReadAsStringAsync());
				records = records.Concat(content.Property("records").Values());
				var q = content.ToObject<SalesforceQueryResult>();
				if(q.IsDone)
					break;
				nextResult = client.GetAsync(q.NextRecordsUrl);
			}
			var r = new SalesforceQueryResult<JToken> {
				IsDone = true,
				Records = records.ToArray(),
			};
			r.TotalSize = r.Records.Length;
			return r;
		}
	}
}
