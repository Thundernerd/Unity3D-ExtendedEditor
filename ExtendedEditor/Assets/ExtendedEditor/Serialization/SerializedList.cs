using System.Collections.Generic;

namespace TNRD.Editor.Serialization {

    public class SerializedList : SerializedBase {

        public List<SerializedBase> Values = new List<SerializedBase>();

        public SerializedList( int id, string type ) : base( id, type ) {
            Mode = ESerializableMode.List;
        }

        public void Add( SerializedBase value ) {
            Values.Add( value );
        }
    }
}