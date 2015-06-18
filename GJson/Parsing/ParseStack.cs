using System;
using System.Collections.Generic;
using System.Globalization;

namespace GJson
{
    partial class Parser
    {
		readonly Stack<JsonValue> _stack = new Stack<JsonValue>();

        public JsonValue Result
        {
            get
            {
                if ( _stack.Count != 1 ) 
                    throw new Exception( "_stack.Count > 0" );

                return _stack.Peek();
            }
        }

        void PushEmpty()
        {
            _stack.Push( new JsonValue() );
        }

        void Push<T>( string s )
        {
			var type = typeof( T );

			if ( type == typeof( string ) )
            {
				PushString( s );
            }
			else if ( type == typeof( double ) )
            {
                _stack.Push( Convert.ToDouble( s, CultureInfo.InvariantCulture ) );
            }
			else if ( type == typeof( bool ) )
            {
                _stack.Push( Convert.ToBoolean( s ) );
            }
        }

		//T Pop<T>()
		//{
		//	var peek = _stack.Peek();
		//	if ( peek == null
		//		|| peek.GetType() != typeof( T ) )
		//		throw new Exception( "Bad stack." );
		//
		//	return ( T )_stack.Pop();
		//}

        void AddItemToObject()
        {
            var value = _stack.Pop();
			var key = _stack.Pop();
			var obj = _stack.Peek();
            obj.Add( key, value );
        }

        void AddItemToArray()
        {
			var value = _stack.Pop();
			var obj = _stack.Peek();
            obj.Add( value );
        }

		void PushString( string s )
        {
            s = s.Substring( 1, s.Length - 2 );

			_stack.Push( s );
        }
    }
}
