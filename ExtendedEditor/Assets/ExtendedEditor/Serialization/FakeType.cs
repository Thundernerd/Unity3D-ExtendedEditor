#if UNITY_EDITOR
using System;

namespace TNRD.Editor.Serialization {

    public class FakeType {

        public string Name;

        public FakeType() { }

        public FakeType( string name ) {
            Name = name;
        }

        public object GetValue() {
            return Type.GetType( Name );
        }
    }
}
#endif