using System;
using System.Collections.Generic;
using System.Globalization;

namespace GJson
{
	class ParserStack<T>
	{
		readonly Stack<T> _stack = new Stack<T>();

        public T Result
        {
            get
            {
                if ( _stack.Count != 1 ) 
                    throw new Exception( "_stack.Count > 0" );

                return _stack.Peek();
            }
        }

		public void Push( T value )
		{
			 _stack.Push( value );
		}
	}

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

		void PushString( string value )
        {
            value = value.Substring( 1, value.Length - 2 );

			_stack.Push( value );
        }

		void PushTrue()
        {
			_stack.Push( true );
        }

		void PushFalse()
        {
			_stack.Push( false );
        }

        void PushDouble( string value )
        {
			_stack.Push( Convert.ToDouble( value, CultureInfo.InvariantCulture ) );
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

		partial void ProductionBegin( ENonTerminal production )
        {
			switch ( production )
	        {
		        case ENonTerminal.Object: PushEmpty(); break;
		        case ENonTerminal.Array: PushEmpty(); break;
	        }
        }

        partial void ProductionEnd( ENonTerminal production )
        {
			switch ( production )
            {
				case ENonTerminal.String : PushString( CurrentToken ); break;
				case ENonTerminal.ObjectItem: AddItemToObject(); break;
				case ENonTerminal.ArrayItem: AddItemToArray(); break;
				case ENonTerminal.Number: PushDouble( CurrentToken ); break;
				case ENonTerminal.True: PushTrue(); break;
				case ENonTerminal.False: PushFalse(); break;
				case ENonTerminal.Null: PushEmpty(); break;
            }
        }
    }
}
