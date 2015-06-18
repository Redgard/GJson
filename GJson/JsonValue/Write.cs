namespace GJson
{
	public partial class JsonValue
	{
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
