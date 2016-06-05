using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SerializedClass : SerializedBase {

    public Dictionary<string, SerializedBase> Values = new Dictionary<string, SerializedBase>();

    public SerializedClass( int id, string type ) : base( id, type ) {
        Mode = ESerializableMode.Class;
    }

    public void Add( string name, SerializedBase value ) {
        Values.Add( name, value );
    }
}
