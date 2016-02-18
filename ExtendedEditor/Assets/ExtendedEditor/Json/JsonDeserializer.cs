using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace TNRD.Editor.Json {

    public class JsonDeserializer {

        public static object Deserialize( string json, Type type ) {
            if ( string.IsNullOrEmpty( json ) ) return null;

            var deserializer = new JsonDeserializer();
            var obj = deserializer.DeserializeObject( json, type );
            return obj;
        }

        public static T Deserialize<T>( string json ) {
            if ( string.IsNullOrEmpty( json ) ) return default( T );

            var deserializer = new JsonDeserializer();
            var obj = deserializer.DeserializeObject<T>( json );
            return obj;
        }

        private List<JsonType> jsonTypes = new List<JsonType>();

        private object DeserializeObject( string json, Type type ) {
            //ReadJsonTypes( ref json );
            return null;
        }

        private T DeserializeObject<T>( string json ) {
            ReadJsonTypes( ref json );
            return default( T );
        }

        private void ReadJsonTypes( ref string json ) {
            var index = json.IndexOf( "$types" );

            var start = json.IndexOf( "[", index ) + 1;
            var end = json.IndexOf( "]", index );

            var jtemp = json.Substring( start, end - start );

            var splits = jtemp.Split( new string[] { "}," }, StringSplitOptions.RemoveEmptyEntries );
            foreach ( var item in splits ) {
                var temp = item.Trim( ' ', '{' );
                var jType = new JsonType();
                var tid = temp.IndexOf( "\"Typename\"" );

                var assembly = temp.Substring( 0, tid ).Trim( ' ', ',' );
                assembly = assembly.Remove( 0, 11 );

                var typename = temp.Substring( tid );
                typename = typename.Remove( 0, 11 );

                jType.Assembly = assembly.Trim( ' ', '\"' );
                jType.Typename = typename.Trim( ' ', '\"' );

                jsonTypes.Add( jType );
            }

            // -1 & +3 for " and ],
            json = json.Remove( index - 1, end - index + 3 );
        }
    }
}