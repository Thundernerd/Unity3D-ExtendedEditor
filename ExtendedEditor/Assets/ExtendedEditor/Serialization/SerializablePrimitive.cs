public class SerializablePrimitive : Serializable {

    public object Value;

    public SerializablePrimitive( int id, object value ) : base( id ) {
        Mode = ESerializableMode.Primitive;
        Value = value;
    }
}
