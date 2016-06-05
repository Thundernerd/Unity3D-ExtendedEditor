public class SerializableEnum : SerializablePrimitive {

    public SerializableEnum( int id, object value ) : base( id, value ) {
        Mode = ESerializableMode.Enum;
    }
}
