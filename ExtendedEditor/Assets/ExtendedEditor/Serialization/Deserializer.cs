#if UNITY_EDITOR
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Reflection;

namespace TNRD.Editor.Serialization {

    public class Deserializer {

        public static T Deserialize<T>( string b64 ) {
            var buffer = Convert.FromBase64String( b64 );
            return Deserialize<T>( buffer );
        }

        public static T Deserialize<T>( byte[] buffer ) {
            var stream = new MemoryStream( buffer );
            var reader = new BinaryReader( stream );

            var deserializer = new Deserializer();
            var deserializedObject = deserializer.DeserializeClass( reader );
            var value = deserializer.ReadClass( deserializedObject );

            return (T)value;
        }

        public static object Deserialize( string b64, Type type ) {
            var buffer = Convert.FromBase64String( b64 );
            return Deserialize( buffer, type );
        }

        public static object Deserialize( byte[] buffer, Type type ) {
            var stream = new MemoryStream( buffer );
            var reader = new BinaryReader( stream );

            var deserializer = new Deserializer();
            var deserializedObject = deserializer.DeserializeClass( reader );
            var value = deserializer.ReadClass( deserializedObject );

            return Convert.ChangeType( value, type );
        }

        #region Reading
        private Dictionary<int, object> deserializedObjects = new Dictionary<int, object>();

        private object ReadClass( SerializedClass value ) {
            if ( value.IsNull ) return null;

            var type = Type.GetType( value.Type );
            object instance = null;

            if ( value.IsReference ) {
                if ( deserializedObjects.ContainsKey( value.ID ) ) {
                    return deserializedObjects[value.ID];
                }
            }

            try {
                instance = Activator.CreateInstance( type, true );
            } catch ( MissingMethodException ) {
                // No constructor
                deserializedObjects.Add( value.ID, null );
                return null;
            }

            deserializedObjects.Add( value.ID, instance );

            var fields = SerializationHelper.GetFields( type, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic )
                            .Where( f =>
                            ( f.IsPublic && f.GetCustomAttributes( typeof( IgnoreSerializationAttribute ), false ).Length == 0 ) ||
                            ( f.IsPrivate && f.GetCustomAttributes( typeof( RequireSerializationAttribute ), false ).Length == 1 ) )
                            .OrderBy( f => f.Name ).ToList();

            var properties = SerializationHelper.GetProperties( type, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic )
                            .Where( p => p.CanRead && p.CanWrite )
                            .Where( p => p.GetCustomAttributes( typeof( IgnoreSerializationAttribute ), false ).Length == 0 )
                            .OrderBy( p => p.Name ).ToList();

            foreach ( var item in value.Values ) {
                var name = item.Key.Substring( 0, item.Key.IndexOf( '|' ) );
                object tValue = null;

                var s = item.Value;
                switch ( s.Mode ) {
                    case ESerializableMode.Primitive:
                        tValue = ReadPrimitive( (SerializedPrimitive)item.Value );
                        break;
                    case ESerializableMode.Enum:
                        tValue = ReadEnum( (SerializedEnum)item.Value );
                        break;
                    case ESerializableMode.List:
                        tValue = ReadList( (SerializedList)item.Value );
                        break;
                    case ESerializableMode.Class:
                        tValue = ReadClass( (SerializedClass)item.Value );
                        break;
                    default:
                        break;
                }

                var field = fields.Where( f => f.Name == name ).FirstOrDefault();
                if ( field != null ) {
                    fields.Remove( field );

                    try {
                        field.SetValue( instance, tValue );
                    } catch ( Exception e ) {
                        Debug.LogException( e );
                    }

                    continue;
                }

                var property = properties.Where( p => p.Name == name ).FirstOrDefault();
                if ( property != null ) {
                    properties.Remove( property );
                    try {
                        property.SetValue( instance, tValue, null );
                    } catch ( Exception e ) {
                        Debug.LogException( e );
                    }

                    continue;
                }
            }

            return instance;
        }

        private object ReadEnum( SerializedEnum value ) {
            return value.Value;
        }

        private object ReadList( SerializedList value ) {
            var type = Type.GetType( value.Type );
            IList instance = null;

            if ( type.IsArray ) {
                instance = Array.CreateInstance( type.GetElementType(), value.Values.Count );

                for ( int i = 0; i < value.Values.Count; i++ ) {
                    var item = value.Values[i];
                    var mode = item.Mode;
                    switch ( mode ) {
                        case ESerializableMode.Primitive:
                            instance[i] = ReadPrimitive( (SerializedPrimitive)item );
                            break;
                        case ESerializableMode.Enum:
                            instance[i] = ReadEnum( (SerializedEnum)item );
                            break;
                        case ESerializableMode.List:
                            instance[i] = ReadList( (SerializedList)item );
                            break;
                        case ESerializableMode.Class:
                            instance[i] = ReadClass( (SerializedClass)item );
                            break;
                        default:
                            break;
                    }
                }
            } else {
                instance = (IList)Activator.CreateInstance( type );

                foreach ( var item in value.Values ) {
                    var mode = item.Mode;
                    switch ( mode ) {
                        case ESerializableMode.Primitive:
                            instance.Add( ReadPrimitive( (SerializedPrimitive)item ) );
                            break;
                        case ESerializableMode.Enum:
                            instance.Add( ReadEnum( (SerializedEnum)item ) );
                            break;
                        case ESerializableMode.List:
                            instance.Add( ReadList( (SerializedList)item ) );
                            break;
                        case ESerializableMode.Class:
                            instance.Add( ReadClass( (SerializedClass)item ) );
                            break;
                        default:
                            break;
                    }
                }
            }

            return instance;
        }

        private object ReadPrimitive( SerializedPrimitive value ) {
            return value.Value;
        }
        #endregion

        #region Deserializing
        private SerializedBase DeserializeDefaults( BinaryReader reader ) {
            var s = new SerializedBase( 0, "" );

            s.ID = reader.ReadInt32();
            s.IsNull = reader.ReadBoolean();
            s.IsReference = reader.ReadBoolean();
            s.Mode = (ESerializableMode)reader.ReadInt32();
            s.Type = reader.ReadString();

            return s;
        }

        private SerializedClass DeserializeClass( BinaryReader reader ) {
            var value = new SerializedClass( DeserializeDefaults( reader ) );
            var count = reader.ReadInt32();

            for ( int i = 0; i < count; i++ ) {
                var name = reader.ReadString();
                var mode = (ESerializableMode)reader.ReadInt32();
                switch ( mode ) {
                    case ESerializableMode.Primitive:
                        value.Add( name, DeserializePrimitive( reader ) );
                        break;
                    case ESerializableMode.Enum:
                        value.Add( name, DeserializeEnum( reader ) );
                        break;
                    case ESerializableMode.List:
                        value.Add( name, DeserializeList( reader ) );
                        break;
                    case ESerializableMode.Class:
                        value.Add( name, DeserializeClass( reader ) );
                        break;
                    default:
                        break;
                }
            }

            return value;
        }

        private SerializedEnum DeserializeEnum( BinaryReader reader ) {
            var value = new SerializedEnum( DeserializeDefaults( reader ) );
            value.Value = reader.ReadInt32();
            return value;
        }

        private SerializedList DeserializeList( BinaryReader reader ) {
            var value = new SerializedList( DeserializeDefaults( reader ) );
            var count = reader.ReadInt32();

            for ( int i = 0; i < count; i++ ) {
                var mode = (ESerializableMode)reader.ReadInt32();
                switch ( mode ) {
                    case ESerializableMode.Primitive:
                        value.Add( DeserializePrimitive( reader ) );
                        break;
                    case ESerializableMode.Enum:
                        value.Add( DeserializeEnum( reader ) );
                        break;
                    case ESerializableMode.List:
                        value.Add( DeserializeList( reader ) );
                        break;
                    case ESerializableMode.Class:
                        value.Add( DeserializeClass( reader ) );
                        break;
                    default:
                        break;
                }
            }

            return value;
        }

        private SerializedPrimitive DeserializePrimitive( BinaryReader reader ) {
            var value = new SerializedPrimitive( DeserializeDefaults( reader ) );
            var type = Type.GetType( value.Type );

            if ( type == typeof( bool ) ) {
                value.Value = reader.ReadBoolean();
            } else if ( type == typeof( byte ) ) {
                value.Value = reader.ReadByte();
            } else if ( type == typeof( char ) ) {
                value.Value = reader.ReadChar();
            } else if ( type == typeof( decimal ) ) {
                value.Value = reader.ReadDecimal();
            } else if ( type == typeof( double ) ) {
                value.Value = reader.ReadDouble();
            } else if ( type == typeof( float ) ) {
                value.Value = reader.ReadSingle();
            } else if ( type == typeof( int ) ) {
                value.Value = reader.ReadInt32();
            } else if ( type == typeof( long ) ) {
                value.Value = reader.ReadInt64();
            } else if ( type == typeof( sbyte ) ) {
                value.Value = reader.ReadSByte();
            } else if ( type == typeof( short ) ) {
                value.Value = reader.ReadInt16();
            } else if ( type == typeof( string ) ) {
                value.Value = reader.ReadString();
            } else if ( type == typeof( uint ) ) {
                value.Value = reader.ReadUInt32();
            } else if ( type == typeof( ulong ) ) {
                value.Value = reader.ReadUInt64();
            } else if ( type == typeof( ushort ) ) {
                value.Value = reader.ReadUInt16();
            } else {
                Debug.LogErrorFormat( "Found an unknown primitive: {0}", type.Name );
            }

            return value;
        }
        #endregion
    }
}
#endif