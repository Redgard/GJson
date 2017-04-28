namespace GJson
{
	public interface IJsonWriter
	{
		void WriteNull();
		void WriteBoolean(bool value);
		void WriteString(string value);
		void WriteReal(double value);
        void WriteLong(long value);
        void WriteObject(JsonValue value);
		void WriteArray(JsonValue value);
	}
}
