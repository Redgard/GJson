using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GJson
{
	public abstract class Converter
	{
		public abstract object Read(JsonValue json);
		public abstract JsonValue Write(object value);
	}

	public class NameAttribute : Attribute
	{
		public readonly string Name;

		public NameAttribute(string name)
		{
		    if (string.IsNullOrEmpty(name))
		        throw new ArgumentNullException(name);
            
            Name = name;
		}
	}

	public class IgnoreAttribute : Attribute
	{
	}

	public class ConverterAttribute : Attribute
	{
		public readonly Type ConverterType;

		public ConverterAttribute(Type type)
		{
			ConverterType = type;
		}
	}

	public class Serializator
	{
        const string _kObjectDictionaryKey = "key";
        const string _kObjectDictionaryValue = "value";

        static readonly Serializator _instance = new Serializator();

	    readonly Dictionary<Type, Converter> _converters = new Dictionary<Type, Converter>();
	    readonly Dictionary<Type, List<SerializeInfo>> _membersInfo = new Dictionary<Type, List<SerializeInfo>>();

		public static JsonValue Serialize(object obj)
		{
			return _instance.SerializeValue(obj);
		}

		public static T Deserialize<T>(JsonValue json)
		{
			return _instance.DeserializeValue<T>(json);
		}

		public static T TryDeserialize<T>(JsonValue json)
		{
			var result = default(T);

			try
			{
				result = Deserialize<T>(json);
			}
			catch
			{
			}

			if (result == null)
				result = Activator.CreateInstance<T>();

			return result;
		}

	    JsonValue SerializeValue(object obj)
		{
			if (obj == null)
				return new JsonValue();

			var type = obj.GetType();

			var converter = FindConverter(type);
			if (converter != null)
				return converter.Write(obj);
            else if (type == typeof(JsonValue))
                return (JsonValue)obj;
            else if (type.IsPrimitive)
				return SerializePrimitive(obj);
			else if (obj is string)
				return (string)obj;
			else if (type.IsEnum)
				return obj.ToString();
			else if (type.IsValueType)
				return SerializeObject(obj);
            else if (typeof(IDictionary).IsAssignableFrom(type))
                return SerializeDictionary(obj as IDictionary);
			else if (typeof(IEnumerable).IsAssignableFrom(type))
				return SerializeArray(obj as IEnumerable);
            else if (type.IsClass)
                return SerializeObject(obj);

            throw new Exception("Unknown type " + type.Name);
		}

	    static JsonValue SerializePrimitive(object obj)
		{
			if (obj is bool)
				return (bool)obj;
			else if (obj is byte)
				return (byte)obj;
			else if (obj is sbyte)
				return (sbyte)obj;
			else if (obj is short)
				return (short)obj;
			else if (obj is ushort)
				return (ushort)obj;
			else if (obj is int)
				return (int)obj;
			else if (obj is uint)
				return (uint)obj;
			else if (obj is long)
				return (long)obj;
			else if (obj is ulong)
				return (ulong)obj;
			else if (obj is char)
				return (char)obj;
			else if (obj is double)
				return (double)obj;
			else if (obj is float)
				return (float)obj;

			throw new Exception("Unknown type");
		}

	    JsonValue SerializeObject(object obj)
		{
			var type = obj.GetType();

			var converter = FindConverter(type);
			if (converter != null)
				return converter.Write(obj);

	        var json = JsonValue.CreateEmptyObject();

			foreach (var member in GetMembers(type))
			{
				if (member.GetCustomAttribute<IgnoreAttribute>() != null)
					continue;

				var name = member.Name;

				var nameAttribute = member.GetCustomAttribute<NameAttribute>();
				if (nameAttribute != null)
					name = nameAttribute.Name;

				var converterAttribute = member.GetCustomAttribute<ConverterAttribute>();
				if (converterAttribute != null)
				{
					converter = Activator.CreateInstance(converterAttribute.ConverterType) as Converter;
					json[name] = converter.Write(member.GetValue(obj));
				}
				else
				{
					json[name] = SerializeValue(member.GetValue(obj));
				}
			}

			return json;
		}

	    bool IsStringDictionary(Type dictionaryType)
	    {
	        return dictionaryType.GetGenericArguments()[0] == typeof(string);
	    }

        JsonValue SerializeDictionary(IDictionary dictionary)
	    {
	        if (IsStringDictionary(dictionary.GetType()))
	            return SerializeStringDictionary(dictionary);
	        else
	            return SerializeObjectDictionary(dictionary);
		}
        
        JsonValue SerializeStringDictionary(IDictionary dictionary)
        {
            var json = JsonValue.CreateEmptyObject();

            foreach (DictionaryEntry entry in dictionary)
                json[(string)entry.Key] = SerializeValue(entry.Value);

            return json;
        }

        JsonValue SerializeObjectDictionary(IDictionary dictionary)
        {
            var json = JsonValue.CreateEmptyArray();
            var jsonAsArray = json.AsArray;

            foreach (DictionaryEntry entry in dictionary)
            {
                var jsonEntry = JsonValue.CreateEmptyObject();
                jsonEntry[_kObjectDictionaryKey] = SerializeValue(entry.Key);
                jsonEntry[_kObjectDictionaryValue] = SerializeValue(entry.Value);

                jsonAsArray.Add(jsonEntry);
            }

            return json;
        }

        JsonValue SerializeArray(IEnumerable enumerable)
        {
            var json = JsonValue.CreateEmptyArray();
            var jsonAsArray = json.AsArray;

            foreach (var item in enumerable)
                jsonAsArray.Add(SerializeValue(item));

            return json;
        }
        
        Converter FindConverter(Type type)
		{
			Converter converter;
			if (_converters.TryGetValue(type, out converter))
				return converter;

			var converterAttribute = GetAttribute<ConverterAttribute>(type);
			if (converterAttribute == null)
				return null;

			converter = Activator.CreateInstance(converterAttribute.ConverterType) as Converter;
			_converters[type] = converter;

			return converter;
		}

	    T GetAttribute<T>(Type type) where T : Attribute
		{
			foreach (var attr in type.GetCustomAttributes(typeof(T), false))
				return attr as T;

			return null;
		}

	    class SerializeInfo
		{
		    readonly FieldInfo _fieldInfo;
		    readonly PropertyInfo _propertyInfo;

			public SerializeInfo(FieldInfo fieldInfo)
			{
				_fieldInfo = fieldInfo;
			}

			public SerializeInfo(PropertyInfo propertyInfo)
			{
				_propertyInfo = propertyInfo;
			}

			public T GetCustomAttribute<T>() where T : Attribute
			{
				if (_fieldInfo != null)
				{
					foreach (var attr in _fieldInfo.GetCustomAttributes(typeof(T), false))
						return attr as T;

					return null;
				}
				else if (_propertyInfo != null)
				{
					foreach (var attr in _propertyInfo.GetCustomAttributes(typeof(T), false))
						return attr as T;

					return null;
				}

				return null;
			}

			public string Name
			{
				get
				{
					if (_fieldInfo != null)
						return _fieldInfo.Name;
					else if (_propertyInfo != null)
						return _propertyInfo.Name;

					return string.Empty;
				}
			}

			public Type MemberType
			{
				get
				{
					if (_fieldInfo != null)
						return _fieldInfo.FieldType;
					else if (_propertyInfo != null)
						return _propertyInfo.PropertyType;

					return null;
				}
			}

			public object GetValue(object owner)
			{
				if (_fieldInfo != null)
					return _fieldInfo.GetValue(owner);
				else if (_propertyInfo != null)
					return _propertyInfo.GetValue(owner, null);

				return null;
			}

			public void SetValue(object value, object owner)
			{
				if (_fieldInfo != null)
					_fieldInfo.SetValue(owner, value);
				else if (_propertyInfo != null)
					_propertyInfo.SetValue(owner, value, null);
			}
		}

	    IEnumerable<SerializeInfo> GetMembers(Type type)
		{
			List<SerializeInfo> result;
			if (_membersInfo.TryGetValue(type, out result))
				return result;

			result = new List<SerializeInfo>();
			_membersInfo[type] = result;

	        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach (var field in fields)
	            result.Add(new SerializeInfo(field));

	        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

	        foreach (var property in properties)
	        {
	            if (property.CanRead
                    && property.CanWrite)
                    result.Add(new SerializeInfo(property));
	        }
            
			return result;
		}

	    T DeserializeValue<T>(JsonValue json)
		{
			if (json == null
			    || json.JsonType == JsonType.Null)
				return default(T);

			var result = DeserializeValue(json, typeof(T));
			if (result == null)
				return default(T);

			return (T)result;
		}

	    object DeserializeValue(JsonValue json, Type type)
		{
			if (json == null
			    || json.JsonType == JsonType.Null)
				return null;

			var converter = FindConverter(type);
			if (converter != null)
				return converter.Read(json);
			else if (type == typeof(JsonValue))
				return json;
            else if (type == typeof(string))
                return (string)json;
            else if (typeof(IDictionary).IsAssignableFrom(type))
                return DeserializeDictionary(json, type);
            else if (typeof(IEnumerable).IsAssignableFrom(type))
                return DeserializeArray(json, type);
			else if (type.IsPrimitive)
				return DeserializePrimitive(json, type);
			else if (type.IsEnum)
				return Enum.Parse(type, json, true);
			else if (type.IsValueType)
				return DeserializeObject(json, type);
			else if (type.IsClass)
				return DeserializeObject(json, type);
            else if (json.JsonType == JsonType.Object)
                return DeserializeObject(json, type);
            else if (json.JsonType == JsonType.Array)
                return DeserializeArray(json, type);

            throw new Exception("Unknown type " + type.Name);
        }

	    object DeserializePrimitive(JsonValue json, Type type)
		{
			if (type == typeof(bool))
				return (bool)json;
			else if (type == typeof(byte))
				return (byte)json;
			else if (type == typeof(sbyte))
				return (sbyte)json;
			else if (type == typeof(short))
				return (short)json;
			else if (type == typeof(ushort))
				return (ushort)json;
			else if (type == typeof(int))
				return (int)json;
			else if (type == typeof(uint))
				return (uint)json;
			else if (type == typeof(long))
				return (long)json;
			else if (type == typeof(ulong))
				return (ulong)json;
			else if (type == typeof(char))
				return (char)json;
			else if (type == typeof(double))
				return (double)json;
			else if (type == typeof(float))
				return (float)json;

            throw new Exception("Unknown type " + type.Name);
        }

	    object DeserializeDictionary(JsonValue json, Type type)
	    {
            var dictionary = (IDictionary)Activator.CreateInstance(type, json.Count);
            
            if (IsStringDictionary(type))
                FillStringDictionary(dictionary, json);
            else
                FillObjectDictionary(dictionary, json);

            return dictionary;
	    }

        void FillStringDictionary(IDictionary dictionary, JsonValue json)
        {
            var valueType = dictionary.GetType().GetGenericArguments()[1];

            foreach (var jsonPair in json.AsObject)
                dictionary.Add(jsonPair.Key, DeserializeValue(jsonPair.Value, valueType));
        }

        void FillObjectDictionary(IDictionary dictionary, JsonValue json)
        {
            var genericArguments = dictionary.GetType().GetGenericArguments();
            var keyType = genericArguments[0];
            var valueType = genericArguments[1];

            foreach (var jsonPair in json.AsArray)
            {
                var jsonPairObject = jsonPair.AsObject;
                dictionary.Add(DeserializeValue(jsonPairObject[_kObjectDictionaryKey], keyType),
                    DeserializeValue(jsonPairObject[_kObjectDictionaryValue], valueType));
            }
        }

        object DeserializeArray(JsonValue json, Type type)
		{
			if (json == null
			    || json.JsonType == JsonType.Null)
				return null;

			// array T[]
			if (type.IsArray)
			{
				var elementType = type.GetElementType();
				var count = json.Count;
				var array = (Array)Activator.CreateInstance(type, count); // Array( int count )

				for (var i = 0; i < count; ++i)
					array.SetValue(DeserializeValue(json[i], elementType), i);

				return array;
			}

			// ICollection[T]
			if (type.IsGenericType && typeof(ICollection).IsAssignableFrom(type))
			{
				var genericParameters = type.GetGenericArguments();
				if (genericParameters.Length == 1)
				{
					var elementType = genericParameters[0];
					var count = json.Count;
					var collection = Activator.CreateInstance(type);

					var addMethod = type.GetMethod("Add", new[] {elementType});

					for (var i = 0; i < count; ++i)
						addMethod.Invoke(collection, new[] {DeserializeValue(json[i], elementType)});

					return collection;
				}
			}

			// IList
			if (typeof(IList).IsAssignableFrom(type))
			{
				Type elementType = null;

				var baseType = type.BaseType;
				while (baseType != typeof(object))
				{
					var genericParameters = baseType.GetGenericArguments();
					if (genericParameters.Length == 1)
					{
						elementType = genericParameters[0];
						break;
					}

					baseType = baseType.BaseType;
				}

				if (elementType != null)
				{
					var count = json.Count;
					var list = (IList)Activator.CreateInstance(type);

					for (var i = 0; i < count; ++i)
						list.Add(DeserializeValue(json[i], elementType));

					return list;
				}
			}

            throw new Exception("Unknown type " + type.Name);
        }

	    object DeserializeObject(JsonValue json, Type type)
		{
			if (json == null
			    || json.JsonType == JsonType.Null)
				return null;

			var instance = Activator.CreateInstance(type);

			foreach (var member in GetMembers(type))
			{
				if (member.GetCustomAttribute<IgnoreAttribute>() != null)
					continue;

				var name = member.Name;

				var nameAttribute = member.GetCustomAttribute<NameAttribute>();
				if (nameAttribute != null)
					name = nameAttribute.Name;

				var value = json[name];
				if (value == null
				    || value.JsonType == JsonType.Null)
					continue;

				var converterAttribute = member.GetCustomAttribute<ConverterAttribute>();
				if (converterAttribute != null)
				{
					var converter = Activator.CreateInstance(converterAttribute.ConverterType) as Converter;
					member.SetValue(converter.Read(value), instance);
				}
				else
				{
					member.SetValue(DeserializeValue(value, member.MemberType), instance);
				}
			}

			return instance;
		}
	}
}
