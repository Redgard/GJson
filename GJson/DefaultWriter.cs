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

                    writer.Write( "null" );

                    break;

                case JsonType.Boolean:

                    writer.Write( ( _bool.GetValueOrDefault() ) ? "true" : "false" );

                    break;

                case JsonType.String:

                    if ( _string == null )
                    {
                        writer.Write( "null" );
                    }
                    else
                    {
                        writer.Write( "\"" );
                        writer.Write( _string );
                        writer.Write( "\"" );
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

                    writer.Write( "{" );

                    bool firstD = true;

                    foreach ( var json in _dict )
                    {
                        if ( !firstD )
                        {
                            writer.Write( "," );
                        }
                        writer.Write( "\"" );
                        writer.Write( json.Key );
                        writer.Write( "\"" );
                        writer.Write( ":" );

                        json.Value.WriteDefault( writer );

                        firstD = false;
                    }

                    writer.Write( "}" );

                    break;

                case JsonType.Array:

                    writer.Write( "[" );

                    bool firstL = true;

                    foreach ( var json in _list )
                    {
                        if ( !firstL )
                        {
                            writer.Write( "," );
                        }

                        json.WriteDefault( writer );

                        firstL = false;
                    }

                    writer.Write( "]" );

                    break;
            }
        }
    }
}
