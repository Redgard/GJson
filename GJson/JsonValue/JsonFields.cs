using System.Collections.Generic;

namespace GJson
{
	public partial class JsonValue
	{
	    JsonType _type = JsonType.Null;
	    string _string;
	    bool? _bool;
	    double? _real;
	    List<JsonValue> _list;
	    Dictionary<string, JsonValue> _dict;

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
