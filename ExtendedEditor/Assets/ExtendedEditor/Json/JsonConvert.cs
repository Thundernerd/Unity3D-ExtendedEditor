using System;

namespace TNRD.Editor.Json {

    public class JsonConvert {

        public static object Deserialize( string json, Type type ) {
            return JsonDeserializer.Deserialize( json, type );
        }

        public static T Deserialize<T>( string json ) where T : class {
            return JsonDeserializer.Deserialize<T>( json );
        }

        public static string Serialize( object value ) {
            return JsonSerializer.Serialize( value );
        }
    }
}
