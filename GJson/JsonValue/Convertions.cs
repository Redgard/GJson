namespace GJson
{
	public partial class JsonValue 
	{
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
	}
}
