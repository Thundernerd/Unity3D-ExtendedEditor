#if UNITY_EDITOR
﻿using System.Collections.Generic;

namespace TNRD.Editor.Serialization {

    public class SerializedList : SerializedBase {

        public List<SerializedBase> Values = new List<SerializedBase>();

        public SerializedList( int id, string type ) : base( id, type ) {
            Mode = ESerializableMode.List;
        }

        public SerializedList( SerializedBase sBase ) : base( sBase.ID, sBase.Type ) {
            IsNull = sBase.IsNull;
            IsReference = sBase.IsReference;
            Mode = sBase.Mode;
        }

        public void Add( SerializedBase value ) {
            Values.Add( value );
        }
    }
}
#endif