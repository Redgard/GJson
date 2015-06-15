using System;
using System.Collections.Generic;
using System.IO;

using GJson;

using Xunit;

namespace GJsonTests
{
	public partial class Tests
	{
		[Fact]
        public void ReadWrite()
        {
			JsonValue v = new JsonValue();

			v["a"] = 32;
			v["d"]["a"] = "gfd";
			v["e"]["a"] = "gfd";
			v["e"]["b"] = 543.423;
			v["e"]["c"] = false;
			v["f"][0] = "gfd";
			v["f"][1]["a"] = 543.423;
			v["f"][1]["b"] = 43242342;
			v["f"][1]["c"] = 1111111.2;
			v["f"][2] = false;

			var s1 = v.ToStringIdent();
			var v2 = JsonValue.Parse( s1 );
			var s2 = v2.ToStringIdent();

			Assert.Equal( s1, s2 );

			var v3 = JsonValue.Parse( File.ReadAllText( "test.json" ) );
			var s3 = v3.ToStringIdent();
			File.WriteAllText( "test_out.json", s3 );

			var v4 = JsonValue.Parse( File.ReadAllText( "test_out.json" ) );
			var s4 = v4.ToStringIdent();

			Assert.Equal( s3, s4 );
		}
	}
}
