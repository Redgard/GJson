using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GJson
{
    public abstract class Converter
    {
        public abstract object Read( JsonValue json );
        public abstract JsonValue Write( object value );
    }

    public class NameAttribute : Attribute
    {
        public readonly string Name;

        public NameAttribute( string name )
        {
            Name = name;
            if ( String.IsNullOrEmpty( Name ) )
            {
                Name = String.Empty;
            }
        }
    }

    public class IgnoreAttribute : Attribute
    {
    }

    public class ConverterAttribute : Attribute
    {
        public readonly Type ConverterType;

        public ConverterAttribute( Type type )
        {
            ConverterType = type;
        }
    }

    public class Serializator
    {
        static readonly Serializator _instance = new Serializator();

        readonly Dictionary<Type, Converter> _converters = new Dictionary<Type, Converter>();
        readonly Dictionary<Type, List<SerializeInfo>> _membersInfo = new Dictionary<Type, List<SerializeInfo>>(); 

        public static JsonValue Serialize( object obj )
        {
            return _instance.SerializeValue( obj );
        }

        public static T Deserialize<T>( JsonValue json )
        {
            return _instance.DeserializeValue<T>( json );
        }

        JsonValue SerializeValue( object obj )
        {
            if ( obj == null )
                return new JsonValue(); 

            Type type = obj.GetType();

            var converter = FindConverter( type );
            if ( converter != null )
            {
                return converter.Write( obj );
            }
            else if ( type.IsPrimitive )
            {
                return SerializePrimitive( obj );
            }
            else if ( obj is string )
            {
                return ( string )obj;
            }
            else if ( type.IsEnum )
            {
                return obj.ToString();
            }
            else if ( type.IsValueType )
            {
                return SerializeObject( obj );
            }
            else if ( typeof ( IEnumerable ).IsAssignableFrom( type ) )
            {
                return SerializeArray( obj as IEnumerable );
            }
            else if ( type.IsClass )
            {
                return SerializeObject( obj );
            }

            throw new Exception( "Unknown type " + type.Name );
        }

        JsonValue SerializePrimitive( object obj )
        {
            if ( obj is Boolean )
            {
                return ( Boolean )obj;
            }
            else if ( obj is Byte )
            {
                return ( Byte )obj;
            }
            else if ( obj is SByte )
            {
                return ( SByte )obj;
            }
            else if ( obj is Int16 )
            {
                return ( Int16 )obj;
            }
            else if ( obj is UInt16 )
            {
                return ( UInt16 )obj;
            }
            else if ( obj is Int32 )
            {
                return ( Int32 )obj;
            }
            else if ( obj is UInt32 )
            {
                return ( UInt32 )obj;
            }
            else if ( obj is Int64 )
            {
                return ( Int64 )obj;
            }
            else if ( obj is UInt64 )
            {
                return ( UInt64 )obj;
            }
            else if ( obj is Char )
            {
                return ( Char )obj;
            }
            else if ( obj is Double )
            {
                return ( Double )obj;
            }
            else if ( obj is Single )
            {
                return ( Single )obj;
            }

            throw new Exception( "Unknown type" );
        }

        JsonValue SerializeObject( object obj )
        {
            if ( obj == null )
                return new JsonValue();

            Type type = obj.GetType();

            Converter converter = FindConverter( type );
            if ( converter != null )
                return converter.Write( obj );

            var json = new JsonValue();
            json.ConvertToObject();

            foreach ( var member in GetMembers( type ) )
            {
                if ( member.GetCustomAttribute<IgnoreAttribute>() != null )
                    continue;

                string name = member.Name;
                
                var nameAttribute = member.GetCustomAttribute<NameAttribute>();
                if ( nameAttribute != null
                    && !String.IsNullOrEmpty( nameAttribute.Name ) )
                {
                    name = nameAttribute.Name;
                }

                var converterAttribute = member.GetCustomAttribute<ConverterAttribute>();
                if ( converterAttribute != null )
                {
                    converter = Activator.CreateInstance( converterAttribute.ConverterType ) as Converter;
                    json[name] = converter.Write( member.GetValue( obj ) );
                }
                else
                {
                    json[name] = SerializeValue( member.GetValue( obj ) );
                }
            }

            return json;
        }

        JsonValue SerializeArray( IEnumerable enumerable )
        {
            if ( enumerable == null )
                return new JsonValue();

            var json = new JsonValue();
            json.ConvertToArray();

            foreach ( object item in enumerable )
            {
                json.Add( SerializeValue( item ) );
            }

            return json;
        }

        Converter FindConverter( Type type )
        {
            Converter converter;
            if ( !_converters.TryGetValue( type, out converter ) )
            {
                var converterAttribute = GetAttribute<ConverterAttribute>( type );
                if ( converterAttribute != null )
                {
                    converter = Activator.CreateInstance( converterAttribute.ConverterType ) as Converter;
                    _converters[type] = converter;
                }
            }

            return converter;
        }

        T GetAttribute<T>( Type type ) where T : Attribute
        {
            foreach ( var attr in type.GetCustomAttributes( typeof( T ), false ) )
            {
                return attr as T;
            }

            return null;
        }

        class SerializeInfo
        {
            readonly FieldInfo _fieldInfo;
            readonly PropertyInfo _propertyInfo;

            public SerializeInfo( FieldInfo fieldInfo )
            {
                _fieldInfo = fieldInfo;
            }

            public SerializeInfo( PropertyInfo propertyInfo )
            {
                _propertyInfo = propertyInfo;
            }

            public T GetCustomAttribute<T>() where T : Attribute 
            {
                if ( _fieldInfo != null )
                {
                    foreach ( var attr in _fieldInfo.GetCustomAttributes( typeof( T ), false ) )
                    {
                        return attr as T;
                    }

                    return null;
                }
                else if ( _propertyInfo != null )
                {
                    foreach ( var attr in _propertyInfo.GetCustomAttributes( typeof( T ), false ) )
                    {
                        return attr as T;
                    }

                    return null;
                }

                return null;
            }

            public string Name
            {
                get
                {
                    if ( _fieldInfo != null )
                    {
                        return _fieldInfo.Name;
                    }
                    else if ( _propertyInfo != null )
                    {
                        return _propertyInfo.Name;
                    }

                    return String.Empty;
                }
            }

            public Type MemberType
            {
                get
                {
                    if ( _fieldInfo != null )
                    {
                        return _fieldInfo.FieldType;
                    }
                    else if ( _propertyInfo != null )
                    {
                        return _propertyInfo.PropertyType;
                    }

                    return null; 
                }
            }

            public object GetValue( object owner )
            {
                if ( _fieldInfo != null )
                {
                    return _fieldInfo.GetValue( owner );
                }
                else if ( _propertyInfo != null )
                {
                    return _propertyInfo.GetValue( owner, null );
                }

                return null;
            }
            
            public void SetValue( object value, object owner )
            {
                if ( _fieldInfo != null )
                {
                    _fieldInfo.SetValue( owner, value );
                }
                else if ( _propertyInfo != null )
                {
                    _propertyInfo.SetValue( owner, value, null );
                }
            }
        }

        List<SerializeInfo> GetMembers( Type type )
        {
            List<SerializeInfo> result;
            if ( _membersInfo.TryGetValue( type, out result ) )
                return result;

            result = new List<SerializeInfo>();
            _membersInfo[type] = result;

            var members = type.GetMembers(
                BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.Instance );

            foreach ( MemberInfo member in members )
            {
                var fieldInfo = member as FieldInfo;
                if ( fieldInfo != null )
                {
                    result.Add( new SerializeInfo( fieldInfo ) );
                }
                else
                {
                    var propertyInfo = member as PropertyInfo;
                    if ( propertyInfo != null
                        && propertyInfo.CanRead
                        && propertyInfo.CanWrite )
                    {
                        result.Add( new SerializeInfo( propertyInfo ) );
                    }
                }
            }
            
            return result;
        }

        T DeserializeValue<T>( JsonValue json )
        {
            if ( json == null
                || json.JsonType == JsonType.Null )
                return default( T );

            object result = DeserializeValue( json, typeof( T ) );
            if ( result == null )
                return default( T );

            return ( T )result;
        }

        object DeserializeValue( JsonValue json, Type type )
        {
            if ( json == null 
                || json.JsonType == JsonType.Null )
                return null;

            var converter = FindConverter( type );
            if ( converter != null )
            {
                return converter.Read( json );
            }
            else if ( json.JsonType == JsonType.Object )
            {
                return DeserializeObject( json, type );
            }
            else if ( json.JsonType == JsonType.Array )
            {
                return DeserializeArray( json, type );
            }
            else if ( type.IsPrimitive )
            {
                return DeserializePrimitive( json, type );
            }
            else if ( type == typeof( string ) )
            {
                return ( string )json;
            }
            else if ( type.IsEnum )
            {
                return Enum.Parse( type, json, true );
            }
            else if ( type.IsValueType )
            {
                return DeserializeObject( json, type );
            }
            else if ( typeof( IEnumerable ).IsAssignableFrom( type ) )
            {
                return DeserializeArray( json, type );
            }
            else if ( type.IsClass )
            {
                return DeserializeObject( json, type );
            }

            throw new Exception( "Unknown type" );
        }

        object DeserializePrimitive( JsonValue json, Type type )
        {
            if ( type == typeof( Boolean ) )
            {
                return ( Boolean )json;
            }
            else if ( type == typeof( Byte ) )
            {
                return ( Byte )json;
            }
            else if ( type == typeof( SByte ) )
            {
                return ( SByte )json;
            }
            else if ( type == typeof( Int16 ) )
            {
                return ( Int16 )json;
            }
            else if ( type == typeof( UInt16 ) )
            {
                return ( UInt16 )json;
            }
            else if ( type == typeof( Int32 ) )
            {
                return ( Int32 )json;
            }
            else if ( type == typeof( UInt32 ) )
            {
                return ( UInt32 )json;
            }
            else if ( type == typeof( Int64 ) )
            {
                return ( Int64 )json;
            }
            else if ( type == typeof( UInt64 ) )
            {
                return ( UInt64 )json;
            }
            else if ( type == typeof( Char ) )
            {
                return ( Char )json;
            }
            else if ( type == typeof( Double ) )
            {
                return ( Double )json;
            }
            else if ( type == typeof( Single ) )
            {
                return ( Single )json;
            }

            throw new Exception( "Unknown type" );
        }

        object DeserializeArray( JsonValue json, Type type )
        {
            if ( json == null 
                || json.JsonType == JsonType.Null )
                return null;

            // array T[]
            if ( type.IsArray )
            {
                Type elementType = type.GetElementType();
                int count = json.Count;
                Array array = ( Array )Activator.CreateInstance( type, count ); // Array( int count )

                for ( int i = 0; i < count; ++i )
                {
                    array.SetValue( DeserializeValue( json[i], elementType ), i );
                }

                return array;
            }

            // ICollection[T]
            if ( type.IsGenericType && typeof( ICollection ).IsAssignableFrom( type ) )
            {
                var genericParameters = type.GetGenericArguments();
                if ( genericParameters.Length == 1 )
                {
                    Type elementType = genericParameters[0];
                    int count = json.Count;
                    object collection = Activator.CreateInstance( type );

                    MethodInfo addMethod = type.GetMethod( "Add", new [] { elementType } );

                    for ( int i = 0; i < count; ++i )
                    {
                        addMethod.Invoke( collection, new[] { DeserializeValue( json[i], elementType ) } );
                    }

                    return collection;
                }
            }

            // IList
            if ( typeof( IList ).IsAssignableFrom( type ) )
            {
                Type elementType = null;

                var baseType = type.BaseType;
                while ( baseType != typeof( object ) )
                {
                    var genericParameters = baseType.GetGenericArguments();
                    if ( genericParameters.Length == 1 )
                    {
                        elementType = genericParameters[0];
                        break;
                    }

                    baseType = baseType.BaseType;
                }

                if ( elementType != null )
                {
                    int count = json.Count;
                    IList list = ( IList )Activator.CreateInstance( type );

                    for ( int i = 0; i < count; ++i )
                    {
                        list.Add( DeserializeValue( json[i], elementType ) );
                    }

                    return list;
                }
            }

            throw new Exception( "Unknown type" );
        }

        object DeserializeObject( JsonValue json, Type type )
        {
            if ( json == null
                 || json.JsonType == JsonType.Null )
                return null;

            object instance = Activator.CreateInstance( type );
            
            foreach ( var member in GetMembers( type ) )
            {
                if ( member.GetCustomAttribute<IgnoreAttribute>() != null )
                    continue;

                string name = member.Name;
                
                var nameAttribute = member.GetCustomAttribute<NameAttribute>();
                if ( nameAttribute != null
                    && !String.IsNullOrEmpty( nameAttribute.Name ) )
                {
                    name = nameAttribute.Name;
                }

                JsonValue value = json[name];
                if ( value == null
                    || value.JsonType == JsonType.Null )
                    continue;

                var converterAttribute = member.GetCustomAttribute<ConverterAttribute>();
                if ( converterAttribute != null )
                {
                    var converter = Activator.CreateInstance( converterAttribute.ConverterType ) as Converter;
                    member.SetValue( converter.Read( value ), instance );
                }
                else
                {
                    member.SetValue( DeserializeValue( value, member.MemberType ), instance );
                }
            }

            return instance;
        }
    }
}
