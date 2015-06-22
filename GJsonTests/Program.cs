using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using GJson;

using Xunit;

namespace GJsonTests
{
    public partial class Tests
    {
		static readonly string _kFilesPath;
 
		static Tests()
		{
			_kFilesPath = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location )
				+ "\\..\\"
				+ Assembly.GetExecutingAssembly().GetName().Name
				+ "\\TestFiles\\";
		}

		static string ReadFile( string fileName )
		{
			return File.ReadAllText( _kFilesPath + fileName );
		}

        static void Main( string[] args )
        {
			JsonValue json = JsonValue.Parse( ReadFile( "test.json" ) );

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
