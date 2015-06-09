using System;
using System.Globalization;
using System.IO;

namespace GJson
{
    public class JsonIdentWriter : StringWriter
    {
        int _ident = 0;

        bool SingleLineArray { set; get; }

        public JsonIdentWriter():
            base( CultureInfo.InvariantCulture )
        {
        }

        public void WriteJson( JsonValue json )
        {
            switch ( json.JsonType )
            {
                case JsonType.Null: WriteNull(); break;
                case JsonType.Boolean: WriteBoolean( json ); break;
                case JsonType.String: WriteString( json ); break;
                case JsonType.Number: WriteNumber( json ); break;
                case JsonType.Object: WriteObject( json ); break;
                case JsonType.Array: WriteArray( json ); break;
            }
        }

        void WriteNull()
        {
			Write( StringConstants.Null );
        }

        void WriteBoolean( bool value )
        {
			Write( ( value ) ? StringConstants.True : StringConstants.False );
        } 
        
        void WriteString( string value )
        {
            if ( value == null )
            {
				Write( StringConstants.Null );
            }
            else
            {
				Write( StringConstants.QuotationMark );
                Write( value );
				Write( StringConstants.QuotationMark );
            }
        }

        void WriteNumber( double value )
        {
            Write( value );
        }

        void WriteIdent()
        {
            for ( int i = 0; i < _ident; ++i )
            {
                Write( StringConstants.Tab );
            }
        }

        void WriteObject( JsonValue json )
        {
            if ( json.Count > 0 )
            {
				Write( StringConstants.CurlyBracketOpen );
                _ident++;

                Write( Environment.NewLine );
                WriteIdent();

                bool firstD = true;

                foreach ( var jsonV in json.AsObject )
                {
                    if ( !firstD )
                    {
                        Write( StringConstants.Comma );
                        Write( Environment.NewLine );
                        WriteIdent();
                    }

					Write( StringConstants.QuotationMark );
                    Write( jsonV.Key );
					Write( StringConstants.QuotationMark + StringConstants.Space + StringConstants.Colon );

                    if ( jsonV.Value.Count > 0 )
                    {
                        Write( Environment.NewLine );
                        WriteIdent();
                    }

                    WriteJson( jsonV.Value );

                    firstD = false;
                }

                Write( Environment.NewLine );
                _ident--;
                WriteIdent();
				Write( StringConstants.CurlyBracketClose );
            }
            else
            {
				Write( StringConstants.CurlyBracketOpen + StringConstants.CurlyBracketClose );
            }
        }

        void WriteArray( JsonValue json )
        {
            if ( json.Count > 0 )
            {
				Write( StringConstants.SquareBracketOpen );
                _ident++;

                if ( !SingleLineArray )
                {
                    Write( Environment.NewLine );
                    WriteIdent();
                }

                bool firstA = true;

                foreach ( var jsonV in json.AsArray )
                {
                    if ( !firstA )
                    {
                        Write( StringConstants.Comma );

                        if ( !SingleLineArray )
                        {
                            Write( Environment.NewLine );
                            WriteIdent();
                        }
                    }

                    WriteJson( jsonV );

                    firstA = false;
                }

                if ( !SingleLineArray )
                {
                    Write( Environment.NewLine );
                }

                _ident--;

                WriteIdent();
				Write( StringConstants.SquareBracketClose );
            }
            else
            {
				Write( StringConstants.SquareBracketOpen + StringConstants.SquareBracketClose );
            }
        }
    }
}
