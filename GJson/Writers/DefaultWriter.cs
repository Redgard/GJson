using System.IO;
using System.Globalization;

namespace GJson
{
	public class DefaultWriter : StringWriter, IJsonWriter
	{
		public DefaultWriter() :
			base(CultureInfo.InvariantCulture)
		{
		}

		public virtual void WriteNull()
		{
			Write(StringConstants.Null);
		}

		public virtual void WriteBoolean(bool value)
		{
			Write((value) ? StringConstants.True : StringConstants.False);
		}

		public virtual void WriteString(string value)
		{
			if (value == null)
			{
				WriteNull();
			}
			else
			{
				Write(StringConstants.QuotationMark);
				Write(value);
				Write(StringConstants.QuotationMark);
			}
		}

	    public void WriteReal(double value)
	    {
	        Write(value);
	    }

	    public void WriteLong(long value)
	    {
            Write(value);
        }
        
		public virtual void WriteObject(JsonValue value)
		{
			Write(StringConstants.CurlyBracketOpen);

			bool firstD = true;

			foreach (var json in value.AsObject)
			{
				if (!firstD)
				{
					Write(StringConstants.Comma);
				}

				Write(StringConstants.QuotationMark);
				Write(json.Key);
				Write(StringConstants.QuotationMark);
				Write(StringConstants.Colon);

				json.Value.Write(this);

				firstD = false;
			}

			Write(StringConstants.CurlyBracketClose);
		}

		public virtual void WriteArray(JsonValue value)
		{
			Write(StringConstants.SquareBracketOpen);

			bool firstL = true;

			foreach (var json in value.AsArray)
			{
				if (!firstL)
				{
					Write(StringConstants.Comma);
				}

				json.Write(this);

				firstL = false;
			}

			Write(StringConstants.SquareBracketClose);
		}
	}
}
