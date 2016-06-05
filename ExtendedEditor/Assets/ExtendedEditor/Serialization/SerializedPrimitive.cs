namespace TNRD.Editor.Serialization {

    public class SerializedPrimitive : SerializedBase {

        public object Value;

        public SerializedPrimitive( int id, string type, object value ) : base( id, type ) {
            Mode = ESerializableMode.Primitive;
            Value = value;
        }
    }
}