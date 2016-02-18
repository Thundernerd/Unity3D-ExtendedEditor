using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TNRD.Editor.Json {

    public class JsonSerializer {

        public static string Serialize( object value ) {
            if ( value == null ) throw new ArgumentNullException( "value" );
            var type = value.GetType();
            var serializer = new JsonSerializer();

            if ( type.IsPrimitive || type.ToString() == "System.String" || type.IsEnum ||
                serializer.IsArray( type ) || serializer.IsDictionary( type ) ) {
                throw new Exception( "Unable to serialize value since it is neither a Class or a Struct" );
            } else if ( type.IsClass || type.IsValueType ) {
                var json = serializer.SerializeObject( value );
                var types = serializer.SerializeTypes( serializer.jsonTypes );
                json = json.Insert( 1, string.Format( "\"$types\":{0},", types ) );
                return json;
            } else {
                throw new Exception( "Oops?" );
            }
        }

        private Type ignoreType;
        private Type propertyType;

        public List<JsonType> jsonTypes = new List<JsonType>();

        private JsonSerializer() {
            ignoreType = typeof( JsonIgnoreAttribute );
            propertyType = typeof( JsonPropertyAttribute );
        }

        private string SerializeObject( object value ) {
            var type = value.GetType();

            var jType = new JsonType() {
                Assembly = type.Assembly.FullName,
                Typename = type.FullName
            };

            var typeId = jsonTypes.IndexOf( jType );

            if ( typeId == -1 ) {
                jsonTypes.Add( jType );
                typeId = jsonTypes.Count - 1;
            }

            var builder = new StringBuilder();
            var writtenAnything = false;

            builder.Append( "{" );

            var fieldsJson = WriteFields( value ).Trim();
            builder.Append( fieldsJson );
            writtenAnything = !string.IsNullOrEmpty( fieldsJson );

            var propertyJson = WriteProperties( value ).Trim();

            if ( writtenAnything && !string.IsNullOrEmpty( propertyJson ) ) {
                builder.Append( "," );
            }
            builder.Append( propertyJson );
            if ( !writtenAnything ) {
                writtenAnything = !string.IsNullOrEmpty( propertyJson );
            }

            if ( writtenAnything ) {
                builder.Insert( 1, string.Format( "\"$typeid\":{0},", typeId ) );
            } else {
                builder.Insert( 1, string.Format( "\"$typeid\":{0}", typeId ) );
            }

            builder.Append( "}" );

            return builder.ToString();
        }

        private string WriteFields( object value ) {
            var type = value.GetType();

            var publicFields = type.GetFields( BindingFlags.Instance | BindingFlags.Public ).Where( m =>
                   m.GetCustomAttributes( ignoreType, false ).Length == 0 ).OrderBy( m => m.Name ).ToList();
            var privateFields = type.GetFields( BindingFlags.Instance | BindingFlags.NonPublic ).Where( m =>
                   m.GetCustomAttributes( propertyType, false ).Length == 1 ).OrderBy( m => m.Name ).ToList();

            var fields = new List<FieldInfo>();
            fields.AddRange( publicFields );
            fields.AddRange( privateFields );

            var builder = new StringBuilder();

            for ( int i = 0; i < fields.Count; i++ ) {
                var finfo = fields[i];
                var ftype = finfo.FieldType;
                var fvalue = finfo.GetValue( value );

                string jname = finfo.Name;
                string jvalue = "";

                jvalue = WriteSimpleValue( fvalue, ftype );

                if ( i < fields.Count - 1 ) {
                    builder.AppendFormat( "\"{0}\":{1}, ", jname, jvalue );
                } else {
                    builder.AppendFormat( "\"{0}\":{1} ", jname, jvalue );
                }
            }

            return builder.ToString();
        }

        private string WriteProperties( object value ) {
            var type = value.GetType();

            var properties = type.GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ).Where( p =>
                p.GetCustomAttributes( ignoreType, false ).Length == 0 ).ToList();

            // Break fast
            if ( properties.Count == 0 ) return "";

            for ( int i = properties.Count - 1; i >= 0; i-- ) {
                var prop = properties[i];
                if ( !prop.CanRead || !prop.CanWrite ) {
                    properties.RemoveAt( i );
                    continue;
                }

                var pGet = prop.GetGetMethod();
                var pSet = prop.GetSetMethod();
                if ( pGet == null || pSet == null ) {
                    properties.RemoveAt( i );
                    continue;
                }

                var isProperty = prop.GetCustomAttributes( propertyType, false ).Length == 1;
                if ( ( !pGet.IsPublic || !pSet.IsPublic ) && !isProperty ) {
                    properties.RemoveAt( i );
                    continue;
                }

                // Remove properties with index for now
                if ( prop.GetIndexParameters().Length > 0 ) {
                    properties.RemoveAt( i );
                    continue;
                }
            }

            // Break fast
            if ( properties.Count == 0 ) return "";

            var builder = new StringBuilder();

            for ( int i = 0; i < properties.Count; i++ ) {
                var pinfo = properties[i];
                var ptype = pinfo.PropertyType;
                var pvalue = pinfo.GetValue( value, null );

                string jname = pinfo.Name;
                string jvalue = "";

                jvalue = WriteSimpleValue( pvalue, ptype );

                if ( i < properties.Count - 1 ) {
                    builder.AppendFormat( "\"{0}\":{1}, ", jname, jvalue );
                } else {
                    builder.AppendFormat( "\"{0}\":{1} ", jname, jvalue );
                }
            }

            return builder.ToString();
        }

        private string WriteArray( object value ) {
            if ( value == null ) return "[]";
            var array = value as IList;

            var builder = new StringBuilder();
            builder.Append( "[" );

            for ( int i = 0; i < array.Count; i++ ) {
                var item = array[i];
                var itemType = item.GetType();
                var itemValue = "";

                itemValue = WriteSimpleValue( item, itemType );

                builder.Append( itemValue );

                if ( i < array.Count - 1 ) {
                    builder.Append( "," );
                }
            }

            builder.Append( "]" );
            return builder.ToString();
        }

        private string WriteDictionary( object value ) {
            if ( value == null ) return "{[],[]}";

            var dict = value as IDictionary;

            var keys = dict.Keys;
            var kEnumerator = keys.GetEnumerator();
            var kBuilder = new StringBuilder( "[" );

            var values = dict.Values;
            var vEnumerator = values.GetEnumerator();
            var vBuilder = new StringBuilder( "[" );

            var builder = new StringBuilder();
            builder.Append( "{" );

            kEnumerator.MoveNext();
            vEnumerator.MoveNext();
            
            for ( int i = 0; i < keys.Count; i++, kEnumerator.MoveNext(), vEnumerator.MoveNext() ) {
                var kCurrent = kEnumerator.Current;
                var kType = kCurrent.GetType();
                var kValue = "";

                var vCurrent = vEnumerator.Current;
                var vType = vCurrent.GetType();
                var vValue = "";

                kValue = WriteSimpleValue( kCurrent, kType );
                vValue = WriteSimpleValue( vCurrent, vType );

                kBuilder.Append( kValue );
                vBuilder.Append( vValue );

                if ( i < keys.Count - 1 ) {
                    kBuilder.Append( "," );
                    vBuilder.Append( "," );
                }
            }

            kBuilder.Append( "]" );
            vBuilder.Append( "]" );

            builder.AppendFormat( "\"keys\":{0}", kBuilder.ToString() );
            builder.Append( "," );
            builder.AppendFormat( "\"values\":{0}", vBuilder.ToString() );

            builder.Append( "}" );
            return builder.ToString();
        }

        private string WriteSimpleValue( object value, Type type ) {
            if ( value == null ) return "null";

            if ( type.IsPrimitive || type.ToString() == "System.String" ) {
                var temp = value.ToString().ToLower();

                switch ( type.ToString() ) {
                    case "System.String":
                    case "System.Char":
                        temp = string.Format( "\"{0}\"", Regex.Escape( temp ) );
                        break;
                }

                return temp;
            } else if ( IsArray( type ) ) {
                return WriteArray( value );
            } else if ( IsDictionary( type ) ) {
                return WriteDictionary( value );
            } else if ( type.IsEnum ) {
                return string.Format( "\"{0}\"", value.ToString().ToLower() );
            } else if ( type.IsClass || type.IsValueType ) {
                return SerializeObject( value );
            } else {
                // Not sure what would happen here
                return "";
            }
        }

        private bool IsArray( Type type ) {
            if ( type.IsArray ) return true;

            var interfaces = type.GetInterfaces();
            foreach ( var item in interfaces ) {
                if ( item.ToString() == "System.Collections.IList" ) {
                    return true;
                }
            }

            return false;
        }

        private bool IsDictionary( Type type ) {
            var interfaces = type.GetInterfaces();
            foreach ( var item in interfaces ) {
                if ( item.ToString() == "System.Collections.IDictionary" ) {
                    return true;
                }
            }

            return false;
        }

        private string SerializeTypes( List<JsonType> types ) {
            var builder = new StringBuilder();
            builder.Append( "[" );

            for ( int i = 0; i < types.Count; i++ ) {
                var item = types[i];

                builder.Append( "{" );
                builder.AppendFormat( "\"Assembly\":\"{0}\",", item.Assembly );
                builder.AppendFormat( "\"Typename\":\"{0}\"", item.Typename );
                builder.Append( "}" );

                if ( i < types.Count - 1 ) {
                    builder.Append( "," );
                }
            }

            builder.Append( "]" );
            return builder.ToString();
        }
    }
}