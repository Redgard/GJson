using System;
using System.IO;
using System.Reflection;
using GJson;

namespace GJsonTests
{
	public partial class Tests
	{
		private static readonly string KFilesPath;

		static Tests()
		{
			KFilesPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
			             + "\\..\\"
			             + Assembly.GetExecutingAssembly().GetName().Name
			             + "\\TestFiles\\";
		}

		public static string ReadFile(string fileName)
		{
			return File.ReadAllText(KFilesPath + fileName);
		}

		private static void Main()
		{
			var json = JsonValue.Parse(ReadFile("test.json"));

			var writer = new IdentWriter();
			writer.SingleLineArray = true;
			writer.IdentString = "    ";

			json.Write(writer);

			Console.WriteLine(writer);

			Console.ReadKey();
		}
	}
}