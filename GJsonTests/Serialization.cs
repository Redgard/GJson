using System;
using System.Collections.Generic;
using GJson;

namespace GJsonTests
{
	public partial class Tests
	{
	    class Myconverter : Converter
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

	    class A
		{
		    MyEnum? _ttt = MyEnum.Q;
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

			[Name("")] string _hh = "fdsfds";

		    enum MyEnum
			{
				Q,
				W,
				E
			}


		    double _gfd = 32.33;

			[Ignore] double _eerr22 = 32.33;

			//[Converter( typeof( BCONVERTER_2 ) )]
		    B _b = new B();

			[Name("C___C")] C _c = new C();

		    List<string> _strlis = new List<string> {"fds", "fd222"};

		    List<C> _clist = new List<C> {new C(), new C(), new C()};
		}

		[Converter(typeof(Bconverter))]
		class B
		{
		}

	    class Bconverter : Converter
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

	    class Bconverter2 : Converter
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

	    class C
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