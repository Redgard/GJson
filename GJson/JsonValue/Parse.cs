using System;
using System.Diagnostics;

namespace GJson
{
	public partial class JsonValue
	{
		public static JsonValue Parse( string text )
		{
            if ( string.IsNullOrEmpty( text ) )
				return new JsonValue();

			var parser = new Parser();
			parser.Errors.Message += x => { throw new JsonParseException( x ); };
			parser.Parse( new Scanner( text ) );

			return parser.Result;
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

	public class JsonParseException : Exception
	{
		public new ParserErrors.Data Data { get; private set; }

		public static bool BreakOnException { get; set; }

		public JsonParseException( ParserErrors.Data data )
		{
			Data = data;

			if ( BreakOnException )
			{
				Debugger.Break();
			}
		}
	}
}
