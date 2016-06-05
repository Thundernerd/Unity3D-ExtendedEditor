using System.Collections.Generic;

public class SerializableList : Serializable {

    public List<Serializable> Values = new List<Serializable>();

    public SerializableList( int id, string type ) : base( id, type ) {
        Mode = ESerializableMode.List;
    }

    public void Add( Serializable value ) {
        Values.Add( value );
    }
}
