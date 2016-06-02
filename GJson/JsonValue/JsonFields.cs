using System.Collections.Generic;

namespace GJson
{
	public partial class JsonValue
	{
		private JsonType _type = JsonType.Null;
		private string _string;
		private bool? _bool;
		private double? _real;
		private List<JsonValue> _list;
		private Dictionary<string, JsonValue> _dict;

		public JsonType JsonType
		{
			get { return _type; }
		}

		public static JsonValue CreateNull()
		{
			return new JsonValue();
		}

		public static JsonValue CreateEmptyArray()
		{
			var json = new JsonValue();
			json.ConvertToArray();
			return json;
		}

		public static JsonValue CreateEmptyObject()
		{
			var json = new JsonValue();
			json.ConvertToObject();
			return json;
		}
	}
}
