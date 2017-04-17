using System;
using System.Collections.Generic;
using System.Globalization;

namespace GJson
{
    class ParserStack<T> : Stack<T>
	{
		public T Result
		{
			get
			{
				if (Count != 1)
					throw new Exception("_stack.Count > 0");

				return Peek();
			}
		}
	}

	partial class Parser
	{
	    readonly ParserStack<JsonValue> _stack = new ParserStack<JsonValue>();

		public JsonValue Result
		{
			get { return _stack.Result; }
		}

	    void PushEmpty()
		{
			_stack.Push(JsonValue.CreateNull());
		}

	    void PushEmptyArray()
		{
			_stack.Push(JsonValue.CreateEmptyArray());
		}

	    void PushEmptyOBject()
		{
			_stack.Push(JsonValue.CreateEmptyObject());
		}

	    void PushString(string value)
		{
			value = value.Substring(1, value.Length - 2);

			_stack.Push(value);
		}

	    void PushTrue()
		{
			_stack.Push(true);
		}

	    void PushFalse()
		{
			_stack.Push(false);
		}

	    void PushDouble(string value)
		{
			_stack.Push(Convert.ToDouble(value, CultureInfo.InvariantCulture));
		}

	    void AddItemToObject()
		{
			var value = _stack.Pop();
			var key = _stack.Pop();
			var obj = _stack.Peek();
			obj.AsObject.Add(key, value);
		}

	    void AddItemToArray()
		{
			var value = _stack.Pop();
			var obj = _stack.Peek();
			obj.AsArray.Add(value);
		}

		partial void ProductionBegin(ENonTerminal production)
		{
			switch (production)
			{
				case ENonTerminal.Object:
					PushEmptyOBject();
					break;

				case ENonTerminal.Array:
					PushEmptyArray();
					break;
			}
		}

		partial void ProductionEnd(ENonTerminal production)
		{
			switch (production)
			{
				case ENonTerminal.String:
					PushString(CurrentToken);
					break;

				case ENonTerminal.Number:
					PushDouble(CurrentToken);
					break;

				case ENonTerminal.True:
					PushTrue();
					break;

				case ENonTerminal.False:
					PushFalse();
					break;

				case ENonTerminal.Null:
					PushEmpty();
					break;

				case ENonTerminal.ObjectItem:
					AddItemToObject();
					break;

				case ENonTerminal.ArrayItem:
					AddItemToArray();
					break;
			}
		}
	}
}
