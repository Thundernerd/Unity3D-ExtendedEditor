namespace TNRD.Editor.Serialization {

    public class SerializedPrimitive : SerializedBase {

        public object Value;

        public SerializedPrimitive( int id, string type, object value ) : base( id, type ) {
            Mode = ESerializableMode.Primitive;
            Value = value;
        }

        public SerializedPrimitive( SerializedBase sBase ) : base( sBase.ID, sBase.Type ) {
            IsNull = sBase.IsNull;
            IsReference = sBase.IsReference;
            Mode = sBase.Mode;
        }
    }
}