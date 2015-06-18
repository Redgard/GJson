using System;
using System.Collections.Generic;
using System.IO;

using GJson;

using Xunit;

namespace GJsonTests
{
    public partial class Tests
    {
        static void Main( string[] args )
        {
			JsonValue json = new JsonValue();

			json["a"].Add( 1 );
			json["a"].Add( 2 );
			json["a"].Add( 3 );
			json["a"].Add( 4 );
			json["a"].Add( 5 );

			var writer = new IdentWriter();
			writer.SingleLineArray = true;

			json.Write( writer );

			Console.WriteLine( writer );

            Console.ReadKey();
        }
    }
}
