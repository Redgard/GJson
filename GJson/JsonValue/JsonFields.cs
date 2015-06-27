using System.Collections.Generic;

namespace GJson
{
	public partial class JsonValue 
	{
		JsonType _type = JsonType.Null;
		string _string;
		bool? _bool;
		float? _real;
		List<JsonValue> _list;
		Dictionary<string, JsonValue> _dict;

		public JsonType JsonType { get { return _type; } }
	}
}
