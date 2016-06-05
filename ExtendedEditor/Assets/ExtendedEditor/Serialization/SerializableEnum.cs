public class SerializableEnum : SerializablePrimitive {

    public SerializableEnum( int id, string type, object value ) : base( id, type, value ) {
        Mode = ESerializableMode.Enum;
    }
}
