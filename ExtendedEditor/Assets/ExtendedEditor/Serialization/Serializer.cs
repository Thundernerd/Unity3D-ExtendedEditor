using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public class Serializer {

    #region Writing
    public static Serializable Serialize( object value ) {
        if ( value == null ) return null;

        var serializer = new Serializer();
        var type = value.GetType();

        if ( type.IsArray ) {
            return serializer.SerializeList( value, type );
        } else if ( type.IsEnum ) {
            return serializer.SerializeEnum( value, type );
        } else if ( type.IsValueType && !type.IsPrimitive ) {
            if ( type == typeof( decimal ) ) {
                return serializer.SerializePrimitive( value, type );
            } else {
                return serializer.SerializeClass( value, type );
            }
        } else if ( type.IsValueType ) {
            return serializer.SerializePrimitive( value, type );
        } else if ( type.IsClass ) {
            if ( type == typeof( string ) ) {
                return serializer.SerializePrimitive( value, type );
            } else if ( type.GetInterfaces().Contains( typeof( IList ) ) ) {
                return serializer.SerializeList( value, type );
            } else {
                return serializer.SerializeClass( value, type );
            }
        }

        return null;
    }

    public static string SerializeToB64( object value ) {
        var buffer = SerializeToBytes( value );
        var b64 = Convert.ToBase64String( buffer );

        return b64;
    }

    public static byte[] SerializeToBytes( object value ) {
        if ( value == null ) {
            return new byte[0];
        }

        var serializer = new Serializer();
        var serializedObject = serializer.SerializeClass( value, value.GetType() );

        var stream = new MemoryStream();
        var writer = new BinaryWriter( stream );
        WriteClass( writer, serializedObject );
        writer.Flush();
        writer.Close();

        var buffer = stream.GetBuffer();
        return buffer;
    }

    private static void WriteDefaults( BinaryWriter writer, Serializable obj ) {
        writer.Write( obj.ID );
        writer.Write( obj.IsNull );
        writer.Write( obj.IsReference );
        writer.Write( (int)obj.Mode );
        writer.Write( obj.Type );
    }

    private static void WriteClass( BinaryWriter writer, SerializableClass obj ) {
        WriteDefaults( writer, obj );
        writer.Write( obj.Values.Count );

        foreach ( var item in obj.Values ) {
            writer.Write( item.Key );
            switch ( item.Value.Mode ) {
                case ESerializableMode.Primitive:
                    WritePrimitive( writer, (SerializablePrimitive)item.Value );
                    break;
                case ESerializableMode.Enum:
                    WriteEnum( writer, (SerializableEnum)item.Value );
                    break;
                case ESerializableMode.List:
                    WriteList( writer, (SerializableList)item.Value );
                    break;
                case ESerializableMode.Class:
                    WriteClass( writer, (SerializableClass)item.Value );
                    break;
                default:
                    break;
            }
        }
    }

    private static void WriteEnum( BinaryWriter writer, SerializableEnum obj ) {
        WriteDefaults( writer, obj );
        writer.Write( (int)obj.Value );
    }

    private static void WriteList( BinaryWriter writer, SerializableList obj ) {
        WriteDefaults( writer, obj );
        writer.Write( obj.Values.Count );

        foreach ( var item in obj.Values ) {
            switch ( item.Mode ) {
                case ESerializableMode.Primitive:
                    WritePrimitive( writer, (SerializablePrimitive)item );
                    break;
                case ESerializableMode.Enum:
                    WriteEnum( writer, (SerializableEnum)item );
                    break;
                case ESerializableMode.List:
                    WriteList( writer, (SerializableList)item );
                    break;
                case ESerializableMode.Class:
                    WriteClass( writer, (SerializableClass)item );
                    break;
                default:
                    break;
            }
        }
    }

    private static void WritePrimitive( BinaryWriter writer, SerializablePrimitive obj ) {
        WriteDefaults( writer, obj );

        var type = Type.GetType( obj.Type );
        if ( type == typeof( bool ) ) {
            writer.Write( (bool)obj.Value );
        } else if ( type == typeof( byte ) ) {
            writer.Write( (byte)obj.Value );
        } else if ( type == typeof( char ) ) {
            writer.Write( (char)obj.Value );
        } else if ( type == typeof( decimal ) ) {
            writer.Write( (decimal)obj.Value );
        } else if ( type == typeof( double ) ) {
            writer.Write( (double)obj.Value );
        } else if ( type == typeof( float ) ) {
            writer.Write( (float)obj.Value );
        } else if ( type == typeof( int ) ) {
            writer.Write( (int)obj.Value );
        } else if ( type == typeof( long ) ) {
            writer.Write( (long)obj.Value );
        } else if ( type == typeof( sbyte ) ) {
            writer.Write( (sbyte)obj.Value );
        } else if ( type == typeof( short ) ) {
            writer.Write( (short)obj.Value );
        } else if ( type == typeof( string ) ) {
            writer.Write( (string)obj.Value );
        } else if ( type == typeof( uint ) ) {
            writer.Write( (uint)obj.Value );
        } else if ( type == typeof( ulong ) ) {
            writer.Write( (ulong)obj.Value );
        } else if ( type == typeof( ushort ) ) {
            writer.Write( (ushort)obj.Value );
        } else {
            UnityEngine.Debug.LogErrorFormat( "Found an unknown primitive: {0}", type.Name );
        }
    }
    #endregion
    
    private struct CSerializable {
        public Serializable Serializable;
        public object Value;

        public CSerializable( Serializable serializable, object value ) {
            Value = value;
            Serializable = serializable;
        }
    }

    #region Serializing
    private List<Serializable> serializables = new List<Serializable>();

    private Dictionary<Type, List<CSerializable>> cSerializables = new Dictionary<Type, List<CSerializable>>();

    private int currentID = 0;
    private int GetNextID() {
        var id = currentID;
        currentID++;
        return id;
    }

    private SerializableClass SerializeClass( object value, Type valueType ) {
        if ( value == null ) {
            return new SerializableClass( GetNextID(), valueType.FullName ) { IsNull = true };
        }

        var obj = new SerializableClass( GetNextID(), valueType.FullName );

        if ( Compare( value, valueType, ref obj.ID ) ) {
            obj.IsReference = true;
            return obj;
        } else {
            AddToComparables( obj, value, valueType );
        }

        var fields = valueType.GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic )
                        .Where( f =>
                        ( f.IsPublic && f.GetCustomAttributes( typeof( IgnoreSerializationAttribute ), false ).Length == 0 ) ||
                        ( f.IsPrivate && f.GetCustomAttributes( typeof( RequireSerializationAttribute ), false ).Length == 1 ) ).ToList();

        var properties = valueType.GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic )
                        .Where( p => p.CanRead && p.CanWrite )
                        .Where( p => p.GetCustomAttributes( typeof( IgnoreSerializationAttribute ), false ).Length == 0 ).ToList();

        for ( int i = 0; i < fields.Count; i++ ) {
            var field = fields[i];
            var type = field.FieldType;
            var tValue = field.GetValue( value );

            if ( type.IsArray ) {
                obj.Add( field.Name, SerializeList( tValue, type ) );
            } else if ( type.IsEnum ) {
                obj.Add( field.Name, SerializeEnum( tValue, type ) );
            } else if ( type.IsValueType && !type.IsPrimitive ) {
                if ( type == typeof( decimal ) ) {
                    obj.Add( field.Name, SerializePrimitive( tValue, type ) );
                } else {
                    obj.Add( field.Name, SerializeClass( tValue, type ) );
                }
            } else if ( type.IsValueType ) {
                obj.Add( field.Name, SerializePrimitive( tValue, type ) );
            } else if ( type.IsClass ) {
                if ( type == typeof( string ) ) {
                    obj.Add( field.Name, SerializePrimitive( tValue, type ) );
                } else if ( type.GetInterfaces().Contains( typeof( IList ) ) ) {
                    obj.Add( field.Name, SerializeList( tValue, type ) );
                } else {
                    obj.Add( field.Name, SerializeClass( tValue, type ) );
                }
            }
        }

        for ( int i = 0; i < properties.Count; i++ ) {
            var property = properties[i];
            var type = property.PropertyType;
            if ( property.GetIndexParameters().Length > 0 ) {
                continue;
            }
            var tValue = property.GetValue( value, null );

            if ( type.IsArray ) {
                obj.Add( property.Name, SerializeList( tValue, type ) );
            } else if ( type.IsEnum ) {
                obj.Add( property.Name, SerializeEnum( tValue, type ) );
            } else if ( type.IsValueType && !type.IsPrimitive ) {
                if ( type == typeof( double ) ) {
                    obj.Add( property.Name, SerializePrimitive( tValue, type ) );
                } else {
                    obj.Add( property.Name, SerializeClass( tValue, type ) );
                }
            } else if ( type.IsValueType ) {
                obj.Add( property.Name, SerializePrimitive( tValue, type ) );
            } else if ( type.IsClass ) {
                if ( type == typeof( string ) ) {
                    obj.Add( property.Name, SerializePrimitive( tValue, type ) );
                } else if ( type.GetInterfaces().Contains( typeof( IList ) ) ) {
                    obj.Add( property.Name, SerializeList( tValue, type ) );
                } else {
                    obj.Add( property.Name, SerializeClass( tValue, type ) );
                }
            }
        }

        return obj;
    }

    private SerializableEnum SerializeEnum( object value, Type valueType ) {
        if ( value == null ) {
            return new SerializableEnum( GetNextID(), valueType.FullName, null ) { IsNull = true };
        }

        return new SerializableEnum( GetNextID(), valueType.FullName, value );
    }

    private SerializableList SerializeList( object value, Type valueType ) {
        if ( value == null ) {
            return new SerializableList( GetNextID(), valueType.FullName ) { IsNull = true };
        }

        var list = new SerializableList( GetNextID(), valueType.FullName );

        if ( Compare( value, valueType, ref list.ID ) ) {
            list.IsReference = true;
            return list;
        } else {
            AddToComparables( list, value, valueType );
        }

        var vList = (IList)value;
        var vCount = vList.Count;
        Type vType = null;

        if ( valueType.IsArray ) {
            vType = valueType.GetElementType();
        } else {
            vType = valueType.GetGenericArguments()[0];
        }

        Func<object, Type, Serializable> method = null;
        bool isClass = false;

        if ( vType.IsArray ) {
            method = SerializeList;
        } else if ( vType.IsEnum ) {
            method = SerializeEnum;
        } else if ( vType.IsValueType && !vType.IsPrimitive ) {
            if ( vType == typeof( decimal ) ) {
                method = SerializePrimitive;
            } else {
                method = SerializeClass;
                isClass = true;
            }
        } else if ( vType.IsValueType ) {
            method = SerializePrimitive;
        } else if ( vType.IsClass ) {
            if ( vType == typeof( string ) ) {
                method = SerializePrimitive;
            } else if ( vType.GetInterfaces().Contains( typeof( IList ) ) ) {
                method = SerializeList;
            } else {
                method = SerializeClass;
                isClass = true;
            }
        }

        for ( int i = 0; i < vCount; i++ ) {
            if ( isClass ) {
                var v = vList[i];
                if ( v == null ) {
                    list.Add( method( v, vType ) );
                } else {
                    list.Add( method( v, v.GetType() ) );
                }
            } else {
                list.Add( method( vList[i], vType ) );
            }
        }

        return list;
    }

    private SerializablePrimitive SerializePrimitive( object value, Type valueType ) {
        return new SerializablePrimitive( GetNextID(), valueType.FullName, value );
    }

    private void AddToComparables( Serializable serializable, object value, Type valueType ) {
        if ( !cSerializables.ContainsKey( valueType ) ) {
            cSerializables.Add( valueType, new List<CSerializable>() );
        }

        cSerializables[valueType].Add( new CSerializable( serializable, value ) );
    }

    private bool Compare( object value, Type valueType, ref int id ) {
        if ( !cSerializables.ContainsKey( valueType ) ) return false;

        var others = cSerializables[valueType];
        foreach ( var item in others ) {
            if ( ReferenceEquals( value, item.Value ) ) {
                id = item.Serializable.ID;
                return true;
            }
        }

        return false;
    }
    #endregion
}
