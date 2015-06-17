using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace GJson
{
	public partial class JsonValue
	{
		public static JsonValue Parse( string text )
		{
			Parser parser = new Parser( new Scanner( text ) );
			parser.Parse();

			if ( parser.errors.Count > 0 )
				throw new Exception( parser.errors.ErrorStream.ToString() );

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
}
