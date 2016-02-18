using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace TNRD.Editor.Json {

    public class JsonSerializer {

        public static string Serialize( object value ) {
            var type = value.GetType();
            if ( type.IsClass ) {
                var serializer = new JsonSerializer();
                var json = serializer.SerializeObject( value );
                var types = serializer.SerializeTypes( serializer.jsonTypes );
                json = json.Insert( 1, string.Format( "\"$types\":{0},", types ) );
                return json;
            } else {

            }

            return "";
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

            var members = type.GetMembers( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ).Where( m =>
                m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property ).ToList();

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

            var jType = new JsonType() {
                Assembly = type.Assembly.FullName,
                Typename = type.FullName
            };

            var typeId = jsonTypes.IndexOf( jType );

            if ( typeId == -1 ) {
                jsonTypes.Add( jType );
                typeId = jsonTypes.Count - 1;
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

                string fname = finfo.Name;
                string fvalue = "";

                // Do weird string check
                if ( ftype.IsPrimitive || ftype.ToString() == "System.String" ) {
                    var temp = finfo.GetValue( value );
                    fvalue = temp == null ? "null" : temp.ToString().ToLower();

                    if ( fvalue != "null" ) {
                        switch ( ftype.ToString() ) {
                            case "System.String":
                            case "System.Char":
                                fvalue = string.Format( "\"{0}\"", Regex.Escape( fvalue ) );
                                break;
                        }
                    }
                } else if ( IsArray( ftype ) ) {
                    fvalue = WriteArray( finfo.GetValue( value ) );
                } else if ( ftype.IsEnum ) {
                    var temp = finfo.GetValue( value );
                    fvalue = temp == null ? "null" : string.Format( "\"{0}\"", temp.ToString().ToLower() );
                } else if ( ftype.IsClass || ftype.IsValueType ) {
                    var temp = finfo.GetValue( value );
                    fvalue = temp == null ? "null" : SerializeObject( temp );
                } else {
                    // Not sure what would happen here
                }

                if ( i < fields.Count - 1 ) {
                    builder.AppendFormat( "\"{0}\":{1}, ", fname, fvalue );
                } else {
                    builder.AppendFormat( "\"{0}\":{1} ", fname, fvalue );
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

                string fname = pinfo.Name;
                string fvalue = "";

                // Do weird string check
                if ( ptype.IsPrimitive || ptype.ToString() == "System.String" ) {
                    var temp = pinfo.GetValue( value, null );
                    fvalue = temp == null ? "null" : temp.ToString().ToLower();

                    if ( fvalue != "null" ) {
                        switch ( ptype.ToString() ) {
                            case "System.String":
                            case "System.Char":
                                fvalue = string.Format( "\"{0}\"", Regex.Escape( fvalue ) );
                                break;
                        }
                    }
                } else if ( IsArray( ptype ) ) {
                    fvalue = WriteArray( pinfo.GetValue( value, null ) );
                } else if ( ptype.IsEnum ) {
                    var temp = pinfo.GetValue( value, null );
                    fvalue = temp == null ? "null" : string.Format( "\"{0}\"", temp.ToString().ToLower() );
                } else if ( ptype.IsClass || ptype.IsValueType ) {
                    var temp = pinfo.GetValue( value, null );
                    fvalue = temp == null ? "null" : SerializeObject( temp );
                } else {
                    // Not sure what would happen here
                }
                
                if ( i < properties.Count - 1 ) {
                    builder.AppendFormat( "\"{0}\":{1}, ", fname, fvalue );
                } else {
                    builder.AppendFormat( "\"{0}\":{1} ", fname, fvalue );
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
                var itemValue = "";
                var itemType = item.GetType();

                if ( itemType.IsPrimitive || itemType.ToString() == "System.String" ) {
                    itemValue = item == null ? "null" : item.ToString().ToLower();

                    if ( itemValue != "null" ) {
                        switch ( itemType.ToString() ) {
                            case "System.String":
                            case "System.Char":
                                itemValue = string.Format( "\"{0}\"", Regex.Escape( itemValue ) );
                                break;
                        }
                    }
                } else if ( IsArray( itemType ) ) {
                    itemValue = WriteArray( item );
                } else if ( itemType.IsEnum ) {
                    itemValue = item == null ? "null" : string.Format( "\"{0}\"", item.ToString().ToLower() );
                } else if ( itemType.IsClass || itemType.IsValueType ) {
                    itemValue = item == null ? "null" : SerializeObject( item );
                } else {
                    // Not sure what would happen here
                }

                builder.Append( itemValue );

                if ( i < array.Count - 1 ) {
                    builder.Append( "," );
                }
            }

            builder.Append( "]" );
            return builder.ToString();
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