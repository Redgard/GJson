using System;
using System.IO;
using System.Reflection;

using GJson;

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
			var json = JsonValue.Parse( ReadFile( "test.json" ) );

			var writer = new IdentWriter();
			writer.SingleLineArray = true;
			writer.IdentString = "    ";

			json.Write( writer );
	
			Console.WriteLine( writer );

            Console.ReadKey();
        }
    }
}
