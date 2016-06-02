using System;
using System.Collections.Generic;
using System.Globalization;

namespace GJson
{
	internal class ParserStack<T> : Stack<T>
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
		private readonly ParserStack<JsonValue> _stack = new ParserStack<JsonValue>();

		public JsonValue Result
		{
			get { return _stack.Result; }
		}

		private void PushEmpty()
		{
			_stack.Push(JsonValue.CreateNull());
		}

		private void PushEmptyArray()
		{
			_stack.Push(JsonValue.CreateEmptyArray());
		}

		private void PushEmptyOBject()
		{
			_stack.Push(JsonValue.CreateEmptyObject());
		}

		private void PushString(string value)
		{
			value = value.Substring(1, value.Length - 2);

			_stack.Push(value);
		}

		private void PushTrue()
		{
			_stack.Push(true);
		}

		private void PushFalse()
		{
			_stack.Push(false);
		}

		private void PushDouble(string value)
		{
			_stack.Push(Convert.ToDouble(value, CultureInfo.InvariantCulture));
		}

		private void AddItemToObject()
		{
			var value = _stack.Pop();
			var key = _stack.Pop();
			var obj = _stack.Peek();
			obj.AsObject.Add(key, value);
		}

		private void AddItemToArray()
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
