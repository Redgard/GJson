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

        void ReplaceObject()
        {
            JsonValue json = _stack.Pop();
            json.ConvertToObject();
            _stack.Push( json );
        }

        void ReplaceArray()
        {
            JsonValue json = _stack.Pop();
            json.ConvertToArray();
            _stack.Push( json );
        }

        void ReplaceEmpty()
        {
            _stack.Pop();
            PushEmpty();
        }

        void Replace<T>( string s )
        {
            _stack.Pop();

            if ( typeof( T ) == typeof( string ) )
            {
                _stack.Push( TrimQuotes( s ) );
            }
            else if ( typeof( T ) == typeof( double ) )
            {
                _stack.Push( Convert.ToDouble( s, CultureInfo.InvariantCulture ) );
            }
            else if ( typeof( T ) == typeof( bool ) )
            {
                _stack.Push( Convert.ToBoolean( s ) );
            }
        }

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

        string TrimQuotes( string s )
        {
            return s.Substring( 1, s.Length - 2 );
        }
    }
}
