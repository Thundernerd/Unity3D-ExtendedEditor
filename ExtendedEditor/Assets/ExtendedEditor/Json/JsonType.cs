using System;

namespace TNRD.Editor.Json {

    public struct JsonType : IEquatable<JsonType> {
        public string Assembly;
        public string Typename;

        public bool Equals( JsonType other ) {
            return other.Assembly == Assembly &&
                other.Typename == Typename;
        }

        private Type type;

        public Type LoadType() {
            if ( type == null ) {
                var assembly = System.Reflection.Assembly.Load( Assembly );
                type = assembly.GetType( Typename );
            }

            return type;
        }
    }
}