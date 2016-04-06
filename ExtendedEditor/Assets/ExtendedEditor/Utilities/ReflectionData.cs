using System;
using System.Reflection;

namespace TNRD.Editor.Utilities {
    [Serializable]
    public class ReflectionData {
        public MethodInfo Initialize;
        public MethodInfo InitializeGUI;
        public MethodInfo Destroy;

        public MethodInfo Focus;
        public MethodInfo LostFocus;

        public MethodInfo Update;
        public MethodInfo InspectorUpdate;

        public MethodInfo GUI;

        public ReflectionData( Type type ) {
            Initialize = type.GetMethod( "InternalInitialize", BindingFlags.Instance | BindingFlags.NonPublic );
            InitializeGUI = type.GetMethod( "InternalInitializeGUI", BindingFlags.Instance | BindingFlags.NonPublic );
            Destroy = type.GetMethod( "InternalDestroy", BindingFlags.Instance | BindingFlags.NonPublic );

            Focus = type.GetMethod( "InternalFocus", BindingFlags.Instance | BindingFlags.NonPublic );
            LostFocus = type.GetMethod( "InternalLostFocus", BindingFlags.Instance | BindingFlags.NonPublic );

            InspectorUpdate = type.GetMethod( "InternalInspectorUpdate", BindingFlags.Instance | BindingFlags.NonPublic );
            Update = type.GetMethod( "InternalUpdate", BindingFlags.Instance | BindingFlags.NonPublic );

            GUI = type.GetMethod( "InternalGUI", BindingFlags.Instance | BindingFlags.NonPublic );
        }
    }
}