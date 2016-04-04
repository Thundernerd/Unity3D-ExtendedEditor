using System;
using System.Collections.Generic;
using System.Reflection;
using TNRD.Editor.Json;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor.Core {

    [Serializable]
    public class ExtendedEditor : EditorWindow, ISerializationCallbackReceiver {

        [Serializable]
        private class ReflectionData {
            public MethodInfo Initialize;
            public MethodInfo InitializeGUI;
            public MethodInfo Destroy;

            public MethodInfo Focus;
            public MethodInfo LostFocus;

            public MethodInfo Update;
            public MethodInfo InspectorUpdate;

            public MethodInfo GUI;

            public ReflectionData() {
                var type = typeof( ExtendedWindow );

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

        public Vector2 Position {
            get { return position.position; }
            set {
                var pos = position;
                pos.position = value;
                position = pos;
            }
        }
        public Vector2 Size {
            get { return position.size; }
            set {
                var pos = position;
                pos.size = value;
                position = pos;
            }
        }

        [JsonProperty]
        private List<ExtendedWindow> windows = new List<ExtendedWindow>();
        [JsonProperty]
        private bool isInitialized;
        [JsonProperty]
        private bool isInitializedGUI;

        private ReflectionData rData;

        private string serializedEditor;
        [SerializeField]
        private List<JsonType> serializedWindowTypes;
        [SerializeField]
        private ScriptableObject dockArea;

        private bool gotCreated = false;

        private void OnInitialize() {
            rData = new ReflectionData();
            isInitialized = true;

            if ( !gotCreated ) {
                var json = EditorPrefs.GetString( name );
                EditorPrefs.DeleteKey( name );
                serializedEditor = json;
            }
        }

        private void OnInitializeGUI() {
            isInitializedGUI = true;
        }

        private void OnDestroy() {
            for ( int i = windows.Count - 1; i >= 0; i-- ) {
                rData.Destroy.Invoke( windows[i], null );
                windows.RemoveAt( i );
            }
        }

        private void OnFocus() {
            for ( int i = windows.Count - 1; i >= 0; i-- ) {
                rData.Focus.Invoke( windows[i], null );
            }
        }

        private void OnLostFocus() {
            for ( int i = windows.Count - 1; i >= 0; i-- ) {
                rData.LostFocus.Invoke( windows[i], null );
            }
        }

        private void OnGUI() {
            if ( !isInitialized ) {
                OnInitialize();
                return;
            }

            if ( !isInitializedGUI ) {
                OnInitializeGUI();
                return;
            }

            var windowsToProcess = new List<ExtendedWindow>( windows );

            BeginWindows();
            for ( int i = windowsToProcess.Count - 1; i >= 0; i-- ) {
                GUI.Window( i, windowsToProcess[i].WindowRect, WindowGUI, windowsToProcess[i].WindowContent );
            }
            EndWindows();
        }

        private void OnInspectorUpdate() {
            var windowsToProcess = new List<ExtendedWindow>( windows );

            for ( int i = 0; i < windowsToProcess.Count; i++ ) {
                rData.InspectorUpdate.Invoke( windowsToProcess[i], null );
            }
        }

        private void Update() {
            if ( !string.IsNullOrEmpty( serializedEditor ) ) {
                var tRect = new Rect( position );
                var editor = (ExtendedEditor)JsonDeserializer.Deserialize( serializedEditor, typeof( ExtendedEditor ) );
                editor.gotCreated = true;

                foreach ( var item in editor.windows ) {
                    item.Editor = editor;
                }

                tRect.y -= 5f;
                editor.position = tRect;

                serializedEditor = string.Empty;

                var t = typeof( EditorWindow );
                var f = t.GetField( "m_Parent", BindingFlags.Instance | BindingFlags.NonPublic );
                var currentDock = (ScriptableObject)f.GetValue( editor );

                var daType = dockArea.GetType();

                var method = daType.GetMethod( "RemoveTab", new[] { typeof( EditorWindow ), typeof( bool ) } );
                method.Invoke( currentDock, new object[] { this, currentDock != dockArea } );

                method = daType.GetMethod( "AddTab", new[] { typeof( EditorWindow ) } );
                method.Invoke( dockArea, new[] { this } );

                Repaint();
            }

            var windowsToProcess = new List<ExtendedWindow>( windows );

            for ( int i = 0; i < windowsToProcess.Count; i++ ) {
                rData.Update.Invoke( windowsToProcess[i], null );
            }
        }

        private void WindowGUI( int id ) {
            if ( id < windows.Count ) {
                rData.GUI.Invoke( windows[id], null );
            }
        }

        public void AddWindow( ExtendedWindow window ) {
            window.Editor = this;

            rData.Initialize.Invoke( window, null );
            windows.Add( window );
        }

        public void RemoveWindow( ExtendedWindow window ) {
            rData.Destroy.Invoke( window, null );
            windows.Remove( window );
        }

        private static ExtendedEditor CreateEditor( params ExtendedWindow[] windows ) {
            var objects = Resources.FindObjectsOfTypeAll<ExtendedEditor>();
            if ( objects.Length > 0 ) {
                foreach ( var editor in objects ) {
                    var eWindows = editor.windows;
                    var foundEditor = true;

                    if ( eWindows.Count == 0 ) foundEditor = false;

                    foreach ( var w1 in eWindows ) {
                        var wType = w1.GetType();
                        var foundWindow = false;
                        foreach ( var w2 in windows ) {
                            if ( wType == w2.GetType() ) {
                                foundWindow = true;
                                break;
                            }
                        }

                        if ( !foundWindow ) {
                            foundEditor = false;
                            break;
                        }
                    }

                    if ( foundEditor ) {
                        editor.Show();
                        return editor;
                    }
                }
            }

            var editorWindow = CreateInstance<ExtendedEditor>();

            if ( editorWindow.rData == null ) {
                editorWindow.rData = new ReflectionData();
            }

            foreach ( var item in windows ) {
                editorWindow.AddWindow( item );
            }

            return editorWindow;
        }

        public static ExtendedEditor CreateEditor( string title, params ExtendedWindow[] windows ) {
            var inst = CreateEditor( windows );

            inst.titleContent = new GUIContent( title );

            var index = 0;
            var id = string.Format( "tnrd_editor_{0}_{1}", title, index );
            while ( EditorPrefs.HasKey( id ) ) {
                index++;
                id = string.Format( "tnrd_editor_{0}_{1}", title, index );
            }

            inst.name = id;
            inst.gotCreated = true;

            return inst;
        }

        private string DoSerialize() {
            var json = JsonSerializer.Serialize( this );

            if ( serializedWindowTypes == null ) {
                serializedWindowTypes = new List<JsonType>();
            }

            serializedWindowTypes.Clear();
            foreach ( var item in windows ) {
                serializedWindowTypes.Add( new JsonType() {
                    Assembly = item.GetType().Assembly.FullName,
                    Typename = item.GetType().FullName
                } );
            }

            var t = typeof( EditorWindow );
            var f = t.GetField( "m_Parent", BindingFlags.Instance | BindingFlags.NonPublic );
            dockArea = (ScriptableObject)f.GetValue( this );

            return json;
        }

        public void OnBeforeSerialize() {
            var json = DoSerialize();
            EditorPrefs.SetString( name, json );
        }

        public void OnAfterDeserialize() {
            var instanceId = name;

            if ( EditorPrefs.HasKey( instanceId ) ) {
                serializedEditor = EditorPrefs.GetString( instanceId );

                if ( isInitialized ) {
                    EditorPrefs.DeleteKey( instanceId );
                }
            }
        }
    }
}