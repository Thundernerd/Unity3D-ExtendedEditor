using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class Serializer {

    public static void Serialize( object value ) {
        if ( value == null ) return;

        var serializer = new Serializer();
        
        var cObj = serializer.SerializeClass( value, value.GetType() );
    }

    public static void Deserialize() {

    }

    private List<Serializable> serializables = new List<Serializable>();

    private SerializableList SerializeList( object value, Type valueType ) {
        if ( value == null ) {
            return new SerializableList( serializables.Count ) { IsNull = true };
        }

        var list = new SerializableList( serializables.Count );

        var vList = (IList)value;
        var vCount = vList.Count;
        Type vType = null;

        if ( valueType.IsArray ) {
            vType = valueType.GetElementType();
        } else {
            vType = valueType.GetGenericArguments()[0];
        }

        Func<object, Type, Serializable> method = null;

        if ( vType.IsArray ) {
            method = SerializeList;
        } else if ( vType.IsEnum ) {
            method = SerializeEnum;
        } else if ( vType.IsValueType && !vType.IsPrimitive ) {
            if ( vType == typeof( decimal ) ) {
                method = SerializePrimitive;
            } else {
                method = SerializeClass;
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
            }
        }

        for ( int i = 0; i < vCount; i++ ) {
            list.Add( method( vList[i], vType ) );
        }

        return list;
    }

    private SerializableEnum SerializeEnum( object value, Type valueType ) {
        if ( value == null ) {
            return new SerializableEnum( serializables.Count, null ) { IsNull = true };
        }

        return new SerializableEnum( serializables.Count, value );
    }

    private SerializablePrimitive SerializePrimitive( object value, Type valueType ) {
        return new SerializablePrimitive( serializables.Count, value );
    }

    private SerializableClass SerializeClass( object value, Type valueType ) {
        if ( value == null ) {
            return new SerializableClass( serializables.Count ) { IsNull = true };
        }

        var obj = new SerializableClass( serializables.Count );

        // Improve the shit out of this
        var otherObjs = serializables.Where( o => o != null && o.GetType() == typeof( SerializableClass ) );
        foreach ( var item in otherObjs ) {
            if ( ReferenceEquals( obj, item ) ) {
                obj.ID = item.ID;
                return obj;
            }
        }


        var fields = valueType.GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic )
                        .Where( f =>
                        ( f.IsPublic && f.GetCustomAttributes( typeof( IgnoreSerializationAttribute ), false ).Length == 0 ) ||
                        ( f.IsPrivate && f.GetCustomAttributes( typeof( SerializeField ), false ).Length == 1 ) ).ToList();

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
}
