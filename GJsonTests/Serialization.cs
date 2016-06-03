using System;
using System.Collections.Generic;
using System.IO;
using GJson;
using Xunit;

namespace GJsonTests
{
	public partial class Tests
	{
		private class Myconverter : Converter
		{
			public override object Read(JsonValue json)
			{
				return -(int)json;
			}

			public override JsonValue Write(object value)
			{
				return (-(int)value);
			}
		}

		private class A
		{
			private MyEnum? _ttt = MyEnum.Q;
			//Dictionary<string, int> oo = new Dictionary<string, int>
			//{
			//    {"q1", 2},
			//    {"w1", 432}
			//};

			[Name("QQQ"), Converter(typeof(Myconverter))]
			public int T
			{
				get { return 1; }
			}

			[Name("")]
			private string _hh = "fdsfds";

			private enum MyEnum
			{
				Q,
				W,
				E
			}


			private double _gfd = 32.33;

			[Ignore]
			private double _eerr22 = 32.33;

			//[Converter( typeof( BCONVERTER_2 ) )]
			private B _b = new B();

			[Name("C___C")]
			private C _c = new C();

			private List<string> _strlis = new List<string> {"fds", "fd222"};

			private List<C> _clist = new List<C> {new C(), new C(), new C()};
		}

		[Converter(typeof(Bconverter))]
		private class B
		{
		}

		private class Bconverter : Converter
		{
			public override object Read(JsonValue json)
			{
				return new B();
			}

			public override JsonValue Write(object value)
			{
				return "B_CUSTOM_";
			}
		}

		private class Bconverter2 : Converter
		{
			public override object Read(JsonValue json)
			{
				return new B();
			}

			public override JsonValue Write(object value)
			{
				return "B_CUSTOM_22";
			}
		}

		private class C
		{
			public int A = 1;
			public int B = 2;
			public string c = "STR__33";
		}

		//[Fact]
		public void Serialization()
		{
			var a = new A();
			var json = Serializator.Serialize(a);

			Console.WriteLine(json.ToStringIdent());

			a = Serializator.Deserialize<A>(json);

			json = Serializator.Serialize(a);

			Console.WriteLine(json.ToStringIdent());

			var tl = new List<C> {new C() {A = 1, B = 2}, new C() {A = 3, B = 4}};
			json = Serializator.Serialize(tl);

			Console.ReadKey();
		}
	}
}