using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

		public SalesforceClient Base => client;

		SalesforceStrongClient(SalesforceClient client, SalesforceActions actions) {
			this.client = client;
			this.actions = actions;
		}

		public async static Task<SalesforceStrongClient> CreateAsync(SalesforceClient sf, string versionUrl) => new SalesforceStrongClient(sf, await sf.GetAsync<SalesforceActions>(versionUrl));
		public async static Task<SalesforceStrongClient> CreateAsync(SalesforceClient sf) => await CreateAsync(sf, (await sf.ListVersionsAsync()).Last().Url);

		public string BuildQueryUrl(string query) => $"{actions.Query}?q={WebUtility.UrlEncode(query)}";

		public async Task<QueryResult<JToken>> QueryAsync(string query) {
			var records = Enumerable.Empty<JToken>();
			return new QueryResult<JToken> {
				TotalSize = await QueryAsync(query, chunk => records = records.Concat(chunk)),
				Records = records,
			};
		}

		public async Task<int> QueryAsync(string query, Action<IEnumerable<JToken>> handleChunk) {
			for(var nextResult = client.GetAsync(BuildQueryUrl(query));;) {
				var response = await nextResult;
				var content = JObject.Parse(await response.Content.ReadAsStringAsync());
				handleChunk(content.Property("records").Values());
				var q = content.ToObject<SalesforceQueryResult>();
				if(q.IsDone)
					return q.TotalSize;
				nextResult = client.GetAsync(q.NextRecordsUrl);
			}
		}
	}
}
