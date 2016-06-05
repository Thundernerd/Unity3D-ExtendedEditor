public class SerializedEnum : SerializedPrimitive {

    public SerializedEnum( int id, string type, object value ) : base( id, type, value ) {
        Mode = ESerializableMode.Enum;
    }
}
