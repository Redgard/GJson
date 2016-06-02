using System;
using GJson;

namespace GJsonCorrect
{
	public class GJsonCorrectFormController
	{
		private readonly GJsonCorrectForm _form;

		public GJsonCorrectFormController(GJsonCorrectForm form)
		{
			_form = form;

			//JsonParseException.BreakOnException = true;
		}

		public void OnTextChanged()
		{
			try
			{
				JsonValue.Parse(_form.InputBox.Text);
				_form.SetStatusText("Successfull");
			}
			catch (JsonParseException jsonException)
			{
				ProcessJsonParseException(jsonException);
			}
		}

		private void ProcessJsonParseException(JsonParseException jsonException)
		{
			var statusText = "Error " + jsonException.Data.Type;

			if (jsonException.Data.Type == ParserErrors.EType.SyntaxError)
			{
				_form.InputBox.SelectionStart = ConvertLineAndColumnToPosition(
					jsonException.Data.Line,
					jsonException.Data.Column);

				_form.InputBox.SelectionLength = 1;

				statusText += " " + jsonException.Data.Line + " " + jsonException.Data.Column;
			}
			else
			{
				_form.InputBox.SelectionLength = 0;
			}

			_form.SetStatusText(statusText);
		}

		private int ConvertLineAndColumnToPosition(int l, int c)
		{
			l--;
			c--;

			int pos = 0;
			int newLineLength = Environment.NewLine.Length;

			for (int i = 0; i < l; ++i)
			{
				pos += _form.InputBox.Lines[i].Length + newLineLength;
			}

			pos += c;

			return pos;
		}
	}
}
