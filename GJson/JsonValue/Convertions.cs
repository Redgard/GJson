using System;

namespace GJson
{
	public partial class JsonValue
	{
		public static implicit operator JsonValue(bool value)
		{
			return new JsonValue {_type = JsonType.Boolean, _long = value ? 1 : 0};
		}

        public static implicit operator JsonValue(byte value)
        {
            return new JsonValue { _type = JsonType.Number, _long = value };
        }
        
        public static implicit operator JsonValue(char value)
		{
			return new JsonValue {_type = JsonType.Boolean, _long = value};
		}
        
        public static implicit operator JsonValue(short value)
		{
			return new JsonValue {_type = JsonType.Number, _long = value};
		}
        
		public static implicit operator JsonValue(int value)
		{
			return new JsonValue {_type = JsonType.Number, _long = value};
		}

		public static implicit operator JsonValue(long value)
		{
			return new JsonValue {_type = JsonType.Number, _long = value};
		}

        public static implicit operator JsonValue(float value)
		{
			return new JsonValue {_type = JsonType.Number, _real = value};
		}

		public static implicit operator JsonValue(double value)
		{
			return new JsonValue {_type = JsonType.Number, _real = value};
		}

		public static implicit operator JsonValue(string value)
		{
			return new JsonValue {_type = JsonType.String, _string = value};
		}

		public static implicit operator bool(JsonValue value)
		{
		    return value.AsBool();
		}

	    bool AsBool()
	    {
            if (_long.HasValue)
                return _long.Value != 0;

            return Math.Abs(_real.GetValueOrDefault()) > double.Epsilon;
        }

        public static implicit operator char(JsonValue value)
        {
            if (value._long.HasValue)
                return (char)value._long.Value;

            return (char)value._real.GetValueOrDefault();
        }

        public static implicit operator short(JsonValue value)
        {
            if (value._long.HasValue)
                return (short)value._long.Value;

            return (short)value._real.GetValueOrDefault();
        }

        public static implicit operator int(JsonValue value)
		{
            if (value._long.HasValue)
                return (int)value._long.Value;

            return (int)value._real.GetValueOrDefault();
        }

		public static implicit operator long(JsonValue value)
		{
            if (value._long.HasValue)
                return value._long.Value;

            return (long)value._real.GetValueOrDefault();
        }

		public static implicit operator double(JsonValue value)
		{
            if (value._long.HasValue)
                return value._long.Value;

            return value._real.GetValueOrDefault();
        }

		public static implicit operator float(JsonValue value)
		{
            if (value._long.HasValue)
                return value._long.Value;

            return (float)value._real.GetValueOrDefault();
        }

		public static implicit operator string(JsonValue value)
		{
			return value._string;
		}
	}
}
