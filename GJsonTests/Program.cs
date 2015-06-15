using System;
using System.Collections.Generic;
using System.IO;

using GJson;

using Xunit;

namespace GJsonTests
{
    public partial class Tests
    {
        class myconverter : Converter
        {
            public override object Read( JsonValue json )
            {
                return -( int )json;
            }

            public override JsonValue Write( object value )
            {
                return ( -( int ) value );
            }
        }

        class A
        {
            MyEnum? ttt = MyEnum.q;
            //Dictionary<string, int> oo = new Dictionary<string, int>
            //{
            //    {"q1", 2},
            //    {"w1", 432}
            //};

            [Name( "QQQ" ), Converter( typeof ( myconverter ) )]
            public int t
            {
                get
                {
                    return 1;
                }
            }

            [Name( "" )]
            string hh = "fdsfds";

            enum MyEnum
            {
                q,w,e
            }


            double gfd = 32.33;

            [Ignore]
            double eerr22 = 32.33;

            //[Converter( typeof( BCONVERTER_2 ) )]
            B __b = new B();

            [Name( "C___C" )]
            C __c_____ = new C();

            List<string> strlis = new List<string> {"fds", "fd222"};

            List<C> Clist = new List<C> { new C(), new C(), new C() };
        }

        [Converter( typeof( BCONVERTER ) )]
        class B
        {
             
        }

        class BCONVERTER : Converter
        {
            public override object Read( JsonValue json )
            {
                return new B();
            }

            public override JsonValue Write( object value )
            {
                return "B_CUSTOM_";
            }
        }

        class BCONVERTER_2 : Converter
        {
            public override object Read( JsonValue json )
            {
                return new B();
            }

            public override JsonValue Write( object value )
            {
                return "B_CUSTOM_22";
            }
        }

        class C
        {
            public int a = 1;
            public int b = 2;
            public string c = "STR__33";
        }

        static void Main( string[] args )
        {
            var a = new A();
            var json = Serializator.Serialize( a );

            Console.WriteLine( json.ToStringIdent() );

            a = Serializator.Deserialize<A>( json );

            json = Serializator.Serialize( a );

            Console.WriteLine( json.ToStringIdent() );

            var tl = new List<C> { new C() { a = 1, b = 2 }, new C() { a = 3, b = 4 } };
            json = Serializator.Serialize( tl );

            Console.ReadKey();
        }
    }
}
