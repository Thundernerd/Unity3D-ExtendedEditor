using System;
using System.Reflection;

namespace TNRD.Editor.Utilities {

    [Serializable]
    public class ReflectionData {
        public MethodInfo Initialize;
        public MethodInfo InitializeGUI;
        public MethodInfo Destroy;
        
        public MethodInfo BeforeSerialize;
        public MethodInfo AfterDeserialize;

        public MethodInfo Focus;
        public MethodInfo LostFocus;

        public MethodInfo Update;
        public MethodInfo InspectorUpdate;

        public MethodInfo GUI;
        public MethodInfo SceneGUI;

        public ReflectionData( Type type ) {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;

            Initialize = type.GetMethod( "InternalInitialize", flags );
            InitializeGUI = type.GetMethod( "InternalInitializeGUI", flags );
            Destroy = type.GetMethod( "InternalDestroy", flags );

            BeforeSerialize = type.GetMethod( "InternalBeforeSerialize", flags );
            AfterDeserialize = type.GetMethod( "InternalAfterDeserialize", flags );

            Focus = type.GetMethod( "InternalFocus", flags );
            LostFocus = type.GetMethod( "InternalLostFocus", flags );

            InspectorUpdate = type.GetMethod( "InternalInspectorUpdate", flags );
            Update = type.GetMethod( "InternalUpdate", flags );

            GUI = type.GetMethod( "InternalGUI", flags );
            SceneGUI = type.GetMethod( "InternalSceneGUI", flags );
        }
    }
}