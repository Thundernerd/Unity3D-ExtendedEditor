using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SerializableClass : Serializable {

    public Dictionary<string, Serializable> Values = new Dictionary<string, Serializable>();

    public SerializableClass( int id, string type ) : base( id, type ) {
        Mode = ESerializableMode.Class;
    }

    public void Add( string name, Serializable value ) {
        Values.Add( name, value );
    }
}
