using System;

namespace GJson
{
	public partial class JsonValue
	{
		public static JsonValue Parse( string text )
		{
			var parser = new Parser();
			parser.Errors.Message += ErrorMessagesDispatcher;
			parser.Parse( new Scanner( text ) );

			return parser.Result;
		}

		static void ErrorMessagesDispatcher( ParserErrors.Data data )
        {
            if ( data.Type == ParserErrors.EType.Error )
            {
                throw new Exception( data.Text );
            }
        }

		public static JsonValue TryParse( string text )
		{
			try
			{
				return Parse( text );
			}
			catch
			{
				return new JsonValue();
			}
		}
	}
}
