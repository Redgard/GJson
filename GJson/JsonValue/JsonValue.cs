using System.Collections.Generic;
using System.Diagnostics;

namespace GJson
{
	public sealed partial class JsonValue
	{
	    void ConvertToArray()
		{
	        if (_type != JsonType.Array)
	        {
	            _type = JsonType.Array;
	            _list = new List<JsonValue>();
	        }
		}

	    void ConvertToObject()
	    {
	        if (_type != JsonType.Object)
	        {
	            _type = JsonType.Object;
	            _dict = new Dictionary<string, JsonValue>();
	        }
	    }

	    public JsonValue this[int index]
		{
			get
			{
				ConvertToArray();

				JsonValue result;
				if (index >= _list.Count)
				{
					result = new JsonValue();
					_list.Add(result);
				}
				else
				{
					result = _list[index];
				}

				return result;
			}
			set
			{
				ConvertToArray();

				if (index >= _list.Count)
				{
					_list.Add(value ?? new JsonValue());
				}
				else
				{
					_list[index] = value ?? new JsonValue();
				}
			}
		}


		public JsonValue this[string key]
		{
			get
			{
				ConvertToObject();

				JsonValue result;
				if (_dict.TryGetValue(key, out result))
					return result;

				result = new JsonValue();
				_dict.Add(key, result);

				return result;
			}
			set
			{
				ConvertToObject();

				_dict[key] = value ?? new JsonValue();
			}
		}

		public int Count
		{
			get
			{
				switch (_type)
				{
					case JsonType.Object:
						return _dict.Count;

					case JsonType.Array:
						return _list.Count;

					default:
						return 0;
				}
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IList<JsonValue> AsArray
		{
			get
			{
				ConvertToArray();

				return _list;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IDictionary<string, JsonValue> AsObject
		{
			get
			{
				ConvertToObject();

				return _dict;
			}
		}
	}
}
