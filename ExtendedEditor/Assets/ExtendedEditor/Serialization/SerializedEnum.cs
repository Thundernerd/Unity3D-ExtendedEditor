namespace TNRD.Editor.Serialization {

    public class SerializedEnum : SerializedPrimitive {

        public SerializedEnum( int id, string type, object value ) : base( id, type, value ) {
            Mode = ESerializableMode.Enum;
        }

        public SerializedEnum( SerializedBase sBase ) : base( sBase.ID, sBase.Type, null ) {
            IsNull = sBase.IsNull;
            IsReference = sBase.IsReference;
            Mode = sBase.Mode;
        }
    }
}