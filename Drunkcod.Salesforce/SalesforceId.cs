using Newtonsoft.Json;
using System;
using System.Text;

namespace Drunkcod.Salesforce
{
	[JsonConverter(typeof(SalesforceIdJsonConverter))]
	public struct SalesforceId
	{
		const string SuffixChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ012345";
		readonly string value;

		private SalesforceId(string value) { this.value = value; }

		public static SalesforceId Parse(string input) {
			if(input.Length == 18)
				return new SalesforceId(input);
			if(input.Length != 15)
				throw new InvalidOperationException();
			var result = new StringBuilder(input);
			var flags = 0;
			var bit = 1;
			foreach(var c in input) {
				if(c >= 'A' && c <= 'Z')
					flags |= bit;
				bit <<= 1;
			}
			result.Append(new[] {
				SuffixChars[flags & 31],
				SuffixChars[flags >> 5 & 31],
				SuffixChars[flags >> 10]
			});
			return new SalesforceId(result.ToString());
		}

		public override string ToString() => value;

		public static bool operator==(SalesforceId lhs, SalesforceId rhs) => lhs.value == rhs.value;
		public static bool operator!=(SalesforceId lhs, SalesforceId rhs) => !(lhs == rhs);
	}

	public class SalesforceIdJsonConverter : JsonConverter
	{
		public override bool CanRead => true;
		public override bool CanWrite => true;
		public override bool CanConvert(Type objectType) => objectType == typeof(SalesforceId);

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
			writer.WriteValue(value.ToString());

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
			SalesforceId.Parse(reader.Value.ToString());
	}
}
