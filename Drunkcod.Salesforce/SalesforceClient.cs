using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Drunkcod.Salesforce
{
	public class SalesforceApiTokenRequest
	{
		public string ConsumerKey;
		public string ConsumerSecret;
		public string Username;
		public string Password;
		public string Token;
	}

	public class SalesforceClient
	{
		readonly JsonSerializer json = new JsonSerializer();
		readonly HttpClient http;

		public static Task<SalesforceTokenResponse> GetTokenAsync(SalesforceApiTokenRequest tokenRequest) =>
			GetTokenAsync("https://login.salesforce.com/services/oauth2/token", tokenRequest);

		public static async Task<SalesforceTokenResponse> GetTokenAsync(string loginUrl, SalesforceApiTokenRequest tokenRequest) {
			var post = $"grant_type=password&client_id={tokenRequest.ConsumerKey}&client_secret={tokenRequest.ConsumerSecret}&username={WebUtility.UrlEncode(tokenRequest.Username)}&password={tokenRequest.Password}{tokenRequest.Token}";
			var http = new HttpClient();
			var payload = new StringContent(post);
			payload.Headers.ContentType = new MediaTypeHeaderValue(@"application/x-www-form-urlencoded");
			var getToken = http.PostAsync(loginUrl, payload);
			return JsonConvert.DeserializeObject<SalesforceTokenResponse>(await (await getToken).Content.ReadAsStringAsync());
		}

		public static SalesforceClient Create(SalesforceTokenResponse login, TimeSpan? timeout = null) {
			var http = new HttpClient();
			if(timeout.HasValue)
				http.Timeout = timeout.Value;
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
