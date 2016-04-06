using System;
using System.Reflection;

namespace TNRD.Editor.Utilities {

    [Serializable]
    public class ReflectionData {
        public MethodInfo Initialize;
        public MethodInfo InitializeGUI;
        public MethodInfo Deserialized;
        public MethodInfo Destroy;

        public MethodInfo Focus;
        public MethodInfo LostFocus;

        public MethodInfo Update;
        public MethodInfo InspectorUpdate;

        public MethodInfo GUI;

        public ReflectionData( Type type ) {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;

            Initialize = type.GetMethod( "InternalInitialize", flags );
            InitializeGUI = type.GetMethod( "InternalInitializeGUI", flags );
            Deserialized = type.GetMethod( "InternalDeserialized", flags );
            Destroy = type.GetMethod( "InternalDestroy", flags );

            Focus = type.GetMethod( "InternalFocus", flags );
            LostFocus = type.GetMethod( "InternalLostFocus", flags );

            InspectorUpdate = type.GetMethod( "InternalInspectorUpdate", flags );
            Update = type.GetMethod( "InternalUpdate", flags );

            GUI = type.GetMethod( "InternalGUI", flags );
        }
    }
}