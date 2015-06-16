using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace GJson
{
    public enum JsonType
    {
        Null,
        Boolean,
        String,
        Number,
        Object,
        Array
    }

    public partial class JsonValue : IList<JsonValue>, IDictionary<string,JsonValue>
    {
        JsonType _type = JsonType.Null;
        bool? _bool;
        long? _int; 
        float? _float; 
        string _string;
        List<JsonValue> _list;
        Dictionary<string, JsonValue> _dict;

        public JsonType JsonType { get { return _type; } }

        public static implicit operator JsonValue( bool value )
        {
            return new JsonValue { _type = JsonType.Boolean, _bool = value };
        }

        public static implicit operator JsonValue( char value )
        {
            return new JsonValue { _type = JsonType.Boolean, _int = value };
        }

        public static implicit operator JsonValue( sbyte value )
        {
            return new JsonValue { _type = JsonType.Number, _int = value };
        }

        public static implicit operator JsonValue( byte value )
        {
            return new JsonValue { _type = JsonType.Number, _int = value };
        }

        public static implicit operator JsonValue( short value )
        {
            return new JsonValue { _type = JsonType.Number, _int = value };
        }

        public static implicit operator JsonValue( ushort value )
        {
            return new JsonValue { _type = JsonType.Number, _int = value };
        }

        public static implicit operator JsonValue( int value )
        {
            return new JsonValue { _type = JsonType.Number, _int = value };
        }

        public static implicit operator JsonValue( uint value )
        {
            return new JsonValue { _type = JsonType.Number, _int = value };
        }

        public static implicit operator JsonValue( long value )
        {
            return new JsonValue { _type = JsonType.Number, _int = value };
        }

        public static implicit operator JsonValue( float value )
        {
            return new JsonValue { _type = JsonType.Number, _float = value };
        }

        public static implicit operator JsonValue( double value )
        {
            return new JsonValue { _type = JsonType.Number, _float = ( float )value };
        }

        public static implicit operator JsonValue( string value )
        {
            return new JsonValue { _type = JsonType.String, _string = value };
        }

        public static implicit operator bool( JsonValue value )
        {
            return value._bool.GetValueOrDefault();
        }

        public static implicit operator int( JsonValue value )
        {
            if ( value._int.HasValue )
                return ( int )value._int.GetValueOrDefault();

            return ( int )value._float.GetValueOrDefault();
        }

        public static implicit operator long( JsonValue value )
        {
            if ( value._int.HasValue )
                return value._int.GetValueOrDefault();

            return ( int )value._float.GetValueOrDefault();
        }

        public static implicit operator double( JsonValue value )
        {
            if ( value._int.HasValue )
                return value._int.GetValueOrDefault();

            return value._float.GetValueOrDefault();
        }

        public static implicit operator string( JsonValue value )
        {
            return value._string;
        }

        void CreateArray()
        {
            _type = JsonType.Array;
            _list = new List<JsonValue>();
        }

        public void ConvertToArray()
        {
            if ( _type != JsonType.Array )
            {
                CreateArray();
            }
        }

        void CreateObject()
        {
            _type = JsonType.Object;
            _dict = new Dictionary<string, JsonValue>();
        }

        public void ConvertToObject()
        {
            if ( _type != JsonType.Object )
            {
                CreateObject();
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
                return ( JsonType == JsonType.Array ) ? ( IList<JsonValue> ) this : new List<JsonValue>();
            }
        }
            
        public IDictionary<string,JsonValue> AsObject
        {
            get
            {
                return ( JsonType == JsonType.Object ) ? ( IDictionary<string,JsonValue> ) this : new Dictionary<string,JsonValue>();
            }
        }    

        public override string ToString()
        {
			return ToString( new DefaultWriter() );
        }

        public string ToStringIdent()
        {
			return ToString( new IdentWriter() );
        }

		public string ToString( IJsonWriter writer )
		{
			Write( writer );
			return writer.ToString();
		}

        public static JsonValue Parse( string text )
        {
            #if !DEBUG
            try
            {
            #endif

                Parser parser = new Parser( new Scanner( text ) );
                parser.Parse();

                if ( parser.errors.Count > 0 )
                    throw new Exception( parser.errors.ErrorStream.ToString() );

                return parser.Result;

            #if !DEBUG
            }
            catch
            {
                return new JsonValue();
            }
            #endif
        }

		public void Write( IJsonWriter writer )
		{
			switch ( JsonType )
			{
				case JsonType.Null:

					writer.WriteNull();

					break;

				case JsonType.Boolean:

					writer.WriteBoolean( _bool.GetValueOrDefault() );

					break;

				case JsonType.String:

					writer.WriteString( _string );

					break;

				case JsonType.Number:

					if ( _int.HasValue )
					{
						writer.WriteNumber( _int.GetValueOrDefault() );
					}
					else
					{
						writer.WriteNumber( _float.GetValueOrDefault() );
					}

					break;

				case JsonType.Object:

					writer.WriteObject( this );

					break;

				case JsonType.Array:

					writer.WriteArray( this );

					break;
			}
		}
    }
}
