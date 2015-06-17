using System;
using System.IO;
using System.Globalization;

namespace GJson
{
	public class DefaultWriter : StringWriter, IJsonWriter
	{
		public DefaultWriter() :
            base( CultureInfo.InvariantCulture )
        {
        }

		public void WriteNull()
		{
			Write( StringConstants.Null );
		}

		public void WriteBoolean( bool value )
		{
			Write( ( value ) ? StringConstants.True : StringConstants.False );
		}

		public void WriteString( string value )
		{
			if ( value == null )
			{
				WriteNull();
			}
			else
			{
				Write( StringConstants.QuotationMark );
				Write( value );
				Write( StringConstants.QuotationMark );
			}
		}

		public void WriteNumber( double value )
		{
			Write( value );
		}

		public void WriteObject( JsonValue value )
		{
			Write( StringConstants.CurlyBracketOpen );

			bool firstD = true;

			foreach ( var json in value.AsObject )
			{
				if ( !firstD )
				{
					Write( StringConstants.Comma );
				}

				Write( StringConstants.QuotationMark );
				Write( json.Key );
				Write( StringConstants.QuotationMark );
				Write( StringConstants.Colon );

				json.Value.Write( this );

				firstD = false;
			}

			Write( StringConstants.CurlyBracketClose );
		}

		public void WriteArray( JsonValue value )
		{
			Write( StringConstants.SquareBracketOpen );

			bool firstL = true;

			foreach ( var json in value.AsArray )
			{
				if ( !firstL )
				{
					Write( StringConstants.Comma );
				}

				json.Write( this );

				firstL = false;
			}

			Write( StringConstants.SquareBracketClose );
		}
	}
}
