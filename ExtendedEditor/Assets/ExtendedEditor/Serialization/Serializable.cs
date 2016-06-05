public class Serializable {

    public int ID;
    public bool IsNull = false;
    public bool IsReference = false;
    public ESerializableMode Mode;
    public string Type;

    public Serializable( int id, string type ) {
        ID = id;
        Type = type;
    }
}
