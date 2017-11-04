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
            public enum MyEnum
            {
                Q,
                W,
                E
            }

            double _gfd = 32.33;

            public object Objk = new object();

	        public JsonValue Jj = JsonValue.Parse("[2]");

            public Dictionary<string, int> Oo = new Dictionary<string, int>
			{
			    {"q1", 2},
			    {"w1", 432}
			};

	        public Dictionary<object, C> Cdict = new Dictionary<object, C>
	        {
	            {
	                new Dictionary<C, C>
	                {
	                    {new C(), new C()}
	                },
	                new C()
	            },
	            {"", new C()}
	        };

            public MyEnum? Ttt = MyEnum.E;
	        public MyEnum? Ttt2 = null;
            
			[Name("QQQ"), Converter(typeof(Myconverter))]
			public int T
			{
				get { return 1; }
			}

			[Name("NEW_STR_nAmE")]
            public string Hh = "fdsfds";
            
			[Ignore]
            public double Eerr22 = 32.33;

			//[Converter( typeof( BCONVERTER_2 ) )]
		    public B Bghg = new B();

			[Name("C___C")]
            public C Ctty = new C();

            public List<string> Strlis = new List<string> {"fds", "fd222"};

            public List<C> Clist = new List<C> {new C(), new C(), new C()};
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
			public int Ai = 1;
			public int Bi = 2;
			public string Cs = "STR__33";
		}

		//[Fact]
		public static void Serialization()
		{
			var a = new A();
			var json = Serializator.Serialize(a);

		    var jsonString = json.ToStringIdent();
            Console.WriteLine(jsonString);

			var deserealizedA = Serializator.Deserialize<A>(json);

			json = Serializator.Serialize(a);

            jsonString = json.ToStringIdent();
            Console.WriteLine(jsonString);

			var tl = new List<C> {new C {Ai = 1, Bi = 2}, new C {Ai = 3, Bi = 4}};
			json = Serializator.Serialize(tl);

			Console.ReadKey();
		}
	}
}
