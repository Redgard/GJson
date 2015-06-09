using System;
using System.IO;

namespace GJson
{
    public partial class JsonValue
    {
        public void WriteTo( TextWriter writer )
        {
        }

        void WriteDefault( TextWriter writer )
        {
            switch ( JsonType )
            {
                case JsonType.Null:

					writer.Write( StringConstants.Null );

                    break;

                case JsonType.Boolean:

					writer.Write( ( _bool.GetValueOrDefault() ) ? StringConstants.True : StringConstants.False );

                    break;

                case JsonType.String:

                    if ( _string == null )
                    {
						writer.Write( StringConstants.Null );
                    }
                    else
                    {
                        writer.Write( StringConstants.QuotationMark );
                        writer.Write( _string );
						writer.Write( StringConstants.QuotationMark );
                    }

                    break;

                case JsonType.Number:

                    if ( _int.HasValue )
                    {
                        writer.Write( _int.GetValueOrDefault() );
                    }
                    else
                    {
                        writer.Write( _float.GetValueOrDefault() );
                    }

                    break;


                case JsonType.Object:

                    writer.Write( StringConstants.CurlyBracketOpen );

                    bool firstD = true;

                    foreach ( var json in _dict )
                    {
                        if ( !firstD )
                        {
                            writer.Write( StringConstants.Comma );
                        }
                        writer.Write( StringConstants.QuotationMark );
                        writer.Write( json.Key );
						writer.Write( StringConstants.QuotationMark );
						writer.Write( StringConstants.Colon );

                        json.Value.WriteDefault( writer );

                        firstD = false;
                    }

					writer.Write( StringConstants.CurlyBracketClose );

                    break;

                case JsonType.Array:

					writer.Write( StringConstants.SquareBracketOpen );

                    bool firstL = true;

                    foreach ( var json in _list )
                    {
                        if ( !firstL )
                        {
							writer.Write( StringConstants.Comma );
                        }

                        json.WriteDefault( writer );

                        firstL = false;
                    }

					writer.Write( StringConstants.SquareBracketClose );

                    break;
            }
        }
    }
}
