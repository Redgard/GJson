using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace GJson
{
	public partial class JsonValue : IList<JsonValue>, IDictionary<string, JsonValue>
	{
		JsonType _type = JsonType.Null;
		bool? _bool;
		long? _int;
		float? _float;
		string _string;
		List<JsonValue> _list;
		Dictionary<string, JsonValue> _dict;

		public JsonType JsonType { get { return _type; } }

		public void ConvertToArray()
		{
			if ( _type != JsonType.Array )
			{
				_type = JsonType.Array;
				_list = new List<JsonValue>();
			}
		}

		public void ConvertToObject()
		{
			if ( _type != JsonType.Object )
			{
				_type = JsonType.Object;
				_dict = new Dictionary<string, JsonValue>();
			}
		}

		public int IndexOf( JsonValue item )
		{
			ConvertToArray();

			return _list.IndexOf( item );
		}

		public void Insert( int index, JsonValue item )
		{
			ConvertToArray();

			_list.Insert( index, item );
		}

		public void RemoveAt( int index )
		{
			if ( _type == JsonType.Array )
			{
				_list.RemoveAt( index );
			}
		}

		public JsonValue this[int index]
		{
			get
			{
				ConvertToArray();

				JsonValue result;
				if ( index >= _list.Count )
				{
					result = new JsonValue();
					_list.Add( result );
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

				if ( index >= _list.Count )
				{
					_list.Add( value ?? new JsonValue() );
				}
				else
				{
					_list[index] = value ?? new JsonValue();
				}
			}
		}

		public void Add( JsonValue item )
		{
			ConvertToArray();

			_list.Add( item ?? new JsonValue() );
		}

		public void Add( KeyValuePair<string, JsonValue> item )
		{
			Add( item.Key, item.Value ?? new JsonValue() );
		}

		public void Clear()
		{
			_type = JsonType.Null;
		}

		public bool Contains( KeyValuePair<string, JsonValue> item )
		{
			return _type == JsonType.Array
                && _list.Contains( item.Value );
		}

		public bool Remove( KeyValuePair<string, JsonValue> item )
		{
			return _type == JsonType.Array
                && _list.Remove( item.Value );
		}

		public bool Contains( JsonValue item )
		{
			return _type == JsonType.Array
                && _list.Contains( item );
		}

		public void CopyTo( JsonValue[] array, int arrayIndex )
		{
			throw new NotImplementedException();
		}

		public void CopyTo( KeyValuePair<string, JsonValue>[] array, int arrayIndex )
		{
			throw new NotImplementedException();
		}

		public bool Remove( JsonValue item )
		{
			return _type == JsonType.Array
                && _list.Remove( item );
		}

		public int Count
		{
			get
			{
				if ( _type == JsonType.Object )
					return _dict.Count;

				if ( _type == JsonType.Array )
					return _list.Count;

				return 0;
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		IEnumerator<JsonValue> IEnumerable<JsonValue>.GetEnumerator()
		{
			ConvertToArray();

			return _list.GetEnumerator();
		}

		IEnumerator<KeyValuePair<string, JsonValue>> IEnumerable<KeyValuePair<string, JsonValue>>.GetEnumerator()
		{
			ConvertToObject();

			return _dict.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			ConvertToObject();

			return _dict.GetEnumerator();
		}

		public bool ContainsKey( string key )
		{
			return _type == JsonType.Object
                && _dict.ContainsKey( key );
		}

		public void Add( string key, JsonValue value )
		{
			ConvertToObject();

			_dict.Add( key, value ?? new JsonValue() );
		}

		public bool Remove( string key )
		{
			return _type == JsonType.Object
                && _dict.Remove( key );
		}

		public bool TryGetValue( string key, out JsonValue value )
		{
			if ( _type == JsonType.Object )
			{
				return _dict.TryGetValue( key, out value );
			}

			value = new JsonValue();
			return false;
		}

		public JsonValue this[string key]
		{
			get
			{
				ConvertToObject();

				JsonValue result;
				if ( !_dict.TryGetValue( key, out result ) )
				{
					result = new JsonValue();
					_dict.Add( key, result );
				}

				return result;
			}
			set
			{
				ConvertToObject();

				_dict[key] = value ?? new JsonValue();
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				ConvertToObject();

				return _dict.Keys;
			}
		}

		public ICollection<JsonValue> Values
		{
			get
			{
				ConvertToObject();

				return _dict.Values;
			}
		}

		public IList<JsonValue> AsArray
		{
			get
			{
				return ( JsonType == JsonType.Array ) ? ( IList<JsonValue> )this : new List<JsonValue>();
			}
		}

		public IDictionary<string, JsonValue> AsObject
		{
			get
			{
				return ( JsonType == JsonType.Object ) ? ( IDictionary<string, JsonValue> )this : new Dictionary<string, JsonValue>();
			}
		}
	}
}
