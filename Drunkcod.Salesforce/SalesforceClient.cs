using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Drunkcod.Salesforce
{
	public class SalesforceClient
	{
		readonly JsonSerializer json = new JsonSerializer();
		readonly HttpClient http;

		public static SalesforceClient Create(SalesforceTokenResponse login) {
			var http = new HttpClient();
			http.BaseAddress = new Uri(login.InstanceUrl);
			http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(login.TokenType, login.AccessToken);
			return new SalesforceClient(http);
		}

		SalesforceClient(HttpClient http) {
			this.http = http;
		}

		public async Task<T> GetAsync<T>(string uri) {
			var get = await http.GetAsync(uri);
			using(var s = await get.Content.ReadAsStreamAsync())
			using(var r = new StreamReader(s))
				return (T)json.Deserialize(r, typeof(T));
		}

		public Task<HttpResponseMessage> GetAsync(string uri) => http.GetAsync(uri);
	}

	public static class SalesforceClientExtensions
	{
		public static Task<SalesforceVersionInfo[]> ListVersionsAsync(this SalesforceClient sf) => sf.GetAsync<SalesforceVersionInfo[]>("services/data");
	}
}
