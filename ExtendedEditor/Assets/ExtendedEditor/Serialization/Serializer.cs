#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TNRD.Editor.Serialization {

    public class Serializer {

        public static SerializedBase Serialize( object value ) {
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

        public static string ToB64( object value ) {
            var buffer = ToBytes( value );
            var b64 = Convert.ToBase64String( buffer );

            return b64;
        }

        public static byte[] ToBytes( object value ) {
            if ( value == null ) {
                return new byte[0];
            }

            var serializer = new Serializer();
            var serializedObject = serializer.SerializeClass( value, value.GetType() );

            var bytes = ByteSerializer.Serialize( serializedObject );
            return bytes;
        }

        private struct CSerializable {
            public SerializedBase Serializable;
            public object Value;

            public CSerializable( SerializedBase serializable, object value ) {
                Value = value;
                Serializable = serializable;
            }
        }

        private Dictionary<Type, List<CSerializable>> cSerializables = new Dictionary<Type, List<CSerializable>>();

        private int currentID = 0;
        private int GetNextID() {
            var id = currentID;
            currentID++;
            return id;
        }

        private SerializedClass SerializeClass( object value, Type valueType ) {
            if ( value == null ) {
                return new SerializedClass( GetNextID(), valueType.AssemblyQualifiedName ) { IsNull = true };
            }

            if ( valueType == typeof( UnityEngine.Texture2D )
                || valueType == typeof( UnityEngine.GUIStyle )
                || valueType == typeof( UnityEngine.GUISkin )
                || valueType == typeof( UnityEngine.GameObject ) ) {
                return new SerializedClass( GetNextID(), valueType.AssemblyQualifiedName ) { IsNull = true };
            }

            var obj = new SerializedClass( GetNextID(), valueType.AssemblyQualifiedName );

            if ( Compare( value, valueType, ref obj.ID ) ) {
                obj.IsReference = true;
                return obj;
            } else {
                AddToComparables( obj, value, valueType );
            }

            var fields = SerializationHelper.GetFields( valueType, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic )
                            .Where( f =>
                            ( f.IsPublic && f.GetCustomAttributes( typeof( IgnoreSerializationAttribute ), false ).Length == 0 ) ||
                            ( f.IsPrivate && f.GetCustomAttributes( typeof( RequireSerializationAttribute ), false ).Length == 1 ) )
                            .OrderBy( f => f.Name ).ToList();

            var properties = SerializationHelper.GetProperties( valueType, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic )
                            .Where( p => p.CanRead && p.CanWrite )
                            .Where( p => p.GetCustomAttributes( typeof( IgnoreSerializationAttribute ), false ).Length == 0 )
                            .OrderBy( p => p.Name ).ToList();

            try {
                for ( int i = 0; i < fields.Count; i++ ) {
                    var field = fields[i];
                    var type = field.FieldType;
                    var tValue = field.GetValue( value );
                    var fname = string.Format( "{0}|{1}", field.Name, i );

                    if ( type == typeof( object ) && tValue != null ) {
                        type = tValue.GetType();
                    }

                    if ( ( type.ToString() == "System.MonoType" && tValue != null ) || type == typeof( Type ) ) {
                        var t = (Type)tValue;
                        var v = "";
                        try {
                            v = t.AssemblyQualifiedName;
                        } catch ( Exception ) { }
                        obj.Add( fname, SerializeClass( new FakeType( v ), typeof( FakeType ) ) );
                    } else if ( type.IsArray() ) {
                        obj.Add( fname, SerializeList( tValue, type ) );
                    } else if ( type.IsEnum ) {
                        obj.Add( fname, SerializeEnum( tValue, type ) );
                    } else if ( type.IsValueType && !type.IsPrimitive ) {
                        if ( type == typeof( decimal ) ) {
                            obj.Add( fname, SerializePrimitive( tValue, type ) );
                        } else {
                            obj.Add( fname, SerializeClass( tValue, type ) );
                        }
                    } else if ( type.IsValueType ) {
                        obj.Add( fname, SerializePrimitive( tValue, type ) );
                    } else if ( type.IsClass ) {
                        if ( type == typeof( string ) ) {
                            obj.Add( fname, SerializePrimitive( tValue, type ) );
                        } else if ( type.IsList() ) {
                            obj.Add( fname, SerializeList( tValue, type ) );
                        } else {
                            obj.Add( fname, SerializeClass( tValue, type ) );
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
                    var pname = string.Format( "{0}|{1}", property.Name, i );

                    if ( type == typeof( object ) && tValue != null ) {
                        type = tValue.GetType();
                    }

                    if ( type.ToString() == "System.MonoType" && tValue != null ) {
                        var t = (Type)tValue;
                        var v = "";
                        try {
                            v = t.AssemblyQualifiedName;
                        } catch ( Exception ) { }
                        obj.Add( pname, SerializeClass( new FakeType( v ), typeof( FakeType ) ) );
                    } else if ( type.IsArray() ) {
                        obj.Add( pname, SerializeList( tValue, type ) );
                    } else if ( type.IsEnum ) {
                        obj.Add( pname, SerializeEnum( tValue, type ) );
                    } else if ( type.IsValueType && !type.IsPrimitive ) {
                        if ( type == typeof( double ) ) {
                            obj.Add( pname, SerializePrimitive( tValue, type ) );
                        } else {
                            obj.Add( pname, SerializeClass( tValue, type ) );
                        }
                    } else if ( type.IsValueType ) {
                        obj.Add( pname, SerializePrimitive( tValue, type ) );
                    } else if ( type.IsClass ) {
                        if ( type == typeof( string ) ) {
                            obj.Add( pname, SerializePrimitive( tValue, type ) );
                        } else if ( type.IsList() ) {
                            obj.Add( pname, SerializeList( tValue, type ) );
                        } else {
                            obj.Add( pname, SerializeClass( tValue, type ) );
                        }
                    }
                }
            } catch ( Exception ) {
                // pretty ugly but doing this to prevent MissingReferenceExcpetion which apparently isn't catchable
                obj.IsNull = true;
                return obj;
            }

            return obj;
        }

        private SerializedEnum SerializeEnum( object value, Type valueType ) {
            if ( value == null ) {
                return new SerializedEnum( GetNextID(), valueType.AssemblyQualifiedName, null ) { IsNull = true };
            }

            return new SerializedEnum( GetNextID(), valueType.AssemblyQualifiedName, value );
        }

        private SerializedList SerializeList( object value, Type valueType ) {
            if ( value == null ) {
                return new SerializedList( GetNextID(), valueType.AssemblyQualifiedName ) { IsNull = true };
            }

            var list = new SerializedList( GetNextID(), valueType.AssemblyQualifiedName );

            if ( Compare( value, valueType, ref list.ID ) ) {
                list.IsReference = true;
                return list;
            } else {
                AddToComparables( list, value, valueType );
            }

            var vList = (IList)value;
            var vCount = vList.Count;
            Type vType = null;

            if ( valueType.IsArray() ) {
                vType = valueType.GetElementType();
                if ( valueType == typeof( Array ) ) {
                    for ( int i = 0; i < vList.Count; i++ ) {
                        var item = vList[i];
                        if ( item != null ) {
                            vType = item.GetType();
                            break;
                        }
                    }
                }
                if ( vType == null ) {
                    vType = typeof( object );
                }
            } else {
                vType = valueType.GetGenericArguments()[0];
            }

            Func<object, Type, SerializedBase> method = null;
            bool isClass = false;

            if ( vType.IsArray() ) {
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
                } else if ( vType.IsList() ) {
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

        private SerializedPrimitive SerializePrimitive( object value, Type valueType ) {
            if ( value == null ) {
                return new SerializedPrimitive( GetNextID(), valueType.AssemblyQualifiedName, null ) { IsNull = true };
            }

            return new SerializedPrimitive( GetNextID(), valueType.AssemblyQualifiedName, value );
        }

        private void AddToComparables( SerializedBase serializable, object value, Type valueType ) {
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

        private class ByteSerializer {

            public static byte[] Serialize( SerializedClass obj ) {
                var s = new ByteSerializer();

                var stream = new MemoryStream();
                var writer = new BinaryWriter( stream );

                s.WriteClass( writer, obj );

                writer.Flush();
                writer.Close();

                var buffer = stream.GetBuffer();
                return buffer;
            }

            private void WriteDefaults( BinaryWriter writer, SerializedBase obj ) {
                writer.Write( obj.ID );
                writer.Write( obj.IsNull );
                writer.Write( obj.IsReference );
                writer.Write( (int)obj.Mode );
                writer.Write( obj.Type );
            }

            private void WriteClass( BinaryWriter writer, SerializedClass obj ) {
                WriteDefaults( writer, obj );
                writer.Write( obj.Values.Count );

                foreach ( var item in obj.Values ) {
                    writer.Write( item.Key );
                    writer.Write( (int)item.Value.Mode );
                    switch ( item.Value.Mode ) {
                        case ESerializableMode.Primitive:
                            WritePrimitive( writer, (SerializedPrimitive)item.Value );
                            break;
                        case ESerializableMode.Enum:
                            WriteEnum( writer, (SerializedEnum)item.Value );
                            break;
                        case ESerializableMode.List:
                            WriteList( writer, (SerializedList)item.Value );
                            break;
                        case ESerializableMode.Class:
                            WriteClass( writer, (SerializedClass)item.Value );
                            break;
                        default:
                            break;
                    }
                }
            }

            private void WriteEnum( BinaryWriter writer, SerializedEnum obj ) {
                WriteDefaults( writer, obj );
                writer.Write( (int)obj.Value );
            }

            private void WriteList( BinaryWriter writer, SerializedList obj ) {
                WriteDefaults( writer, obj );
                writer.Write( obj.Values.Count );

                foreach ( var item in obj.Values ) {
                    writer.Write( (int)item.Mode );
                    switch ( item.Mode ) {
                        case ESerializableMode.Primitive:
                            WritePrimitive( writer, (SerializedPrimitive)item );
                            break;
                        case ESerializableMode.Enum:
                            WriteEnum( writer, (SerializedEnum)item );
                            break;
                        case ESerializableMode.List:
                            WriteList( writer, (SerializedList)item );
                            break;
                        case ESerializableMode.Class:
                            WriteClass( writer, (SerializedClass)item );
                            break;
                        default:
                            break;
                    }
                }
            }

            private void WritePrimitive( BinaryWriter writer, SerializedPrimitive obj ) {
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
                    writer.Write( obj.IsNull ? "" : (string)obj.Value );
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
        }
    }
}
#endif