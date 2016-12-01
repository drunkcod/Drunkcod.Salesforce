using Newtonsoft.Json;
using System.Collections.Generic;

namespace Drunkcod.Salesforce
{
	public class SalesforceQueryResult
	{
		[JsonProperty("totalSize")] public int TotalSize;
		[JsonProperty("done")] public bool IsDone;
		[JsonProperty("nextRecordsUrl")] public string NextRecordsUrl;
	}

	public class SalesforceQueryResult<T> : SalesforceQueryResult
	{
		[JsonProperty("records")] public T[] Records;
	}

	public class QueryResult<T>
	{
		[JsonProperty("totalSize")] public int TotalSize;
		[JsonProperty("records")] public IEnumerable<T> Records;
	}
}
