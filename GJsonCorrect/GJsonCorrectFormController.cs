using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GJson;

namespace GJsonCorrect
{
	public class GJsonCorrectFormController
	{
		GJsonCorrectForm _form;

		public GJsonCorrectFormController( GJsonCorrectForm form )
		{
			_form = form;

			//JsonParseException.BreakOnException = true;
		}

		public void OnTextChanged()
		{
			try
			{
				var json = JsonValue.Parse( _form.InputBox.Text );
				_form.SetStatusText( "Successfull" );
			}
			catch ( JsonParseException jsonException )
			{
				ProcessJsonParseException( jsonException );
			}
			catch ( Exception exception )
			{
				throw exception;
			}
		}

		void ProcessJsonParseException( JsonParseException jsonException )
		{
			var statusText = "Error " + jsonException.Data.Type;

			if ( jsonException.Data.Type == ParserErrors.EType.SyntaxError )
			{
				_form.InputBox.SelectionStart = ConvertLineAndColumnToPosition(
					jsonException.Data.Line,
					jsonException.Data.Column );

				_form.InputBox.SelectionLength = 1;

				statusText += " " + jsonException.Data.Line + " " + jsonException.Data.Column;
			}
			else
			{
				_form.InputBox.SelectionLength = 0;
			}

			_form.SetStatusText( statusText );
		}

		int ConvertLineAndColumnToPosition( int l, int c )
		{
			l--;
			c--;

			int pos = 0;
			int newLineLength = Environment.NewLine.Length;

			for ( int i = 0; i < l; ++i )
			{
				pos += _form.InputBox.Lines[i].Length + newLineLength;
			}

			pos += c;

			return pos;
		}
	}
}
