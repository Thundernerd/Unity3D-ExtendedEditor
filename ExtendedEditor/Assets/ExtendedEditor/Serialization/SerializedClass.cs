using System.Collections.Generic;

namespace TNRD.Editor.Serialization {

    public class SerializedClass : SerializedBase {

        public Dictionary<string, SerializedBase> Values = new Dictionary<string, SerializedBase>();

        public SerializedClass( int id, string type ) : base( id, type ) {
            Mode = ESerializableMode.Class;
        }

        public void Add( string name, SerializedBase value ) {
            Values.Add( name, value );
        }
    }
}