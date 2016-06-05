using System.Collections.Generic;

public class SerializableList : Serializable {

    public List<Serializable> Values = new List<Serializable>();

    public SerializableList( int id ) : base( id ) {
        Mode = ESerializableMode.List;
    }

    public void Add( Serializable value ) {
        Values.Add( value );
    }
}
