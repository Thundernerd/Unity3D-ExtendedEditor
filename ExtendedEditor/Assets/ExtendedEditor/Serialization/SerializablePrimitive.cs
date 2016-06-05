public class SerializablePrimitive : Serializable {

    public object Value;

    public SerializablePrimitive( int id, string type, object value ) : base( id, type ) {
        Mode = ESerializableMode.Primitive;
        Value = value;
    }
}
