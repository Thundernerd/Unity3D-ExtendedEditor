#if UNITY_EDITOR
﻿namespace TNRD.Editor.Serialization {

    public class SerializedBase {

        public int ID;
        public bool IsNull = false;
        public bool IsReference = false;
        public ESerializableMode Mode;
        public string Type;

        public SerializedBase( int id, string type ) {
            ID = id;
            Type = type;
        }
    }
}
#endif