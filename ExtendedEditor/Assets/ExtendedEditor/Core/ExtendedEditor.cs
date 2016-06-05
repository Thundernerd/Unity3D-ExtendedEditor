using System.Collections.Generic;
using System.Linq;
using TNRD.Editor.Serialization;
using TNRD.Editor.Utilities;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor.Core {

    public sealed class ExtendedEditor : EditorWindow, ISerializationCallbackReceiver {

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

        public ExtendedAssets Assets = new ExtendedAssets();

        [IgnoreSerialization]
        public ExtendedInput Input = new ExtendedInput();

        [RequireSerialization]
        private List<ExtendedWindow> windows = new List<ExtendedWindow>();
        [RequireSerialization]
        private bool isInitialized;
        [RequireSerialization]
        private bool isInitializedGUI;

        private ExtendedPopup popup = null;

        // Windows that will be added and initialized _after_ creating and initializing the editor
        // Otherwise some editor vars might not be initialized properly
        private List<ExtendedWindow> windowsToAdd = new List<ExtendedWindow>();

        private static ReflectionData rData = new ReflectionData( typeof( ExtendedWindow ) );
        private static ReflectionData pData = new ReflectionData( typeof( ExtendedPopup ) );

        [RequireSerialization]
        private int windowIDs = 0;

        private int draggingID = -1;

        private void OnInitialize() {
            isInitialized = true;

            if ( windowsToAdd.Count > 0 ) {
                foreach ( var item in windowsToAdd ) {
                    AddWindow( item );
                }

                windowsToAdd.Clear();
            }
        }

        private void OnInitializeGUI() {
            isInitializedGUI = true;
        }

        private void OnDeserialized() {
            Input = new ExtendedInput();

            foreach ( var item in windows ) {
                item.Editor = this;
                rData.Deserialized.Invoke( item, null );
            }

            Repaint();
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

            Input.OnGUI();

            var windowsToProcess = new List<ExtendedWindow>( windows );

            BeginWindows();
            if ( popup != null ) {
                popup.WindowRect = GUI.Window( popup.WindowID, popup.WindowRect, PopupGUI, popup.WindowContent, ExtendedGUI.DefaultWindowStyle );
            }

            for ( int i = windowsToProcess.Count - 1; i >= 0; i-- ) {
                var wnd = windowsToProcess[i];
                GUIStyle wStyle = GetWindowStyle( wnd.WindowStyle );

                if ( wStyle == null ) {
                    wnd.WindowRect = GUI.Window( wnd.WindowID, wnd.WindowRect, WindowGUI, wnd.WindowContent );
                } else {
                    wnd.WindowRect = GUI.Window( wnd.WindowID, wnd.WindowRect, WindowGUI, wnd.WindowContent, wStyle );
                }

                if ( wnd.WindowSettings.Resizable ) {
                    var r = new Rect(
                        wnd.WindowRect.x + wnd.WindowRect.width - 7.5f,
                        wnd.WindowRect.y + wnd.WindowRect.height - 7.5f,
                        15, 15 );

                    EditorGUIUtility.AddCursorRect( r, MouseCursor.ResizeUpLeft );
                    if ( r.Contains( Event.current.mousePosition ) && Event.current.type == EventType.MouseDrag ) {
                        draggingID = wnd.WindowID;
                    }
                }
            }
            EndWindows();

            if ( draggingID != -1 && Event.current.type == EventType.MouseDrag && Event.current.button == 0 ) {
                var wnd = windowsToProcess.Where( w => w.WindowID == draggingID ).FirstOrDefault();
                wnd.Size += Event.current.delta;
                Repaint();
            }

            if ( Input.ButtonReleased( EMouseButton.Left ) ) {
                draggingID = -1;
            }
        }

        private GUIStyle GetWindowStyle( EWindowStyle style ) {
            switch ( style ) {
                case EWindowStyle.Default:
                    return ExtendedGUI.DefaultWindowStyle;
                case EWindowStyle.DefaultUnity:
                    return null;
                case EWindowStyle.NoToolbarDark:
                    return ExtendedGUI.DarkNoneWindowStyle;
                case EWindowStyle.NoToolbarLight:
                    return GUIStyle.none;
            }

            return null;
        }

        private void OnInspectorUpdate() {
            var windowsToProcess = new List<ExtendedWindow>( windows );

            for ( int i = 0; i < windowsToProcess.Count; i++ ) {
                rData.InspectorUpdate.Invoke( windowsToProcess[i], null );
            }
        }

        private void Update() {
            if ( popup != null ) {
                pData.Update.Invoke( popup, null );
            }

            var windowsToProcess = new List<ExtendedWindow>( windows );

            for ( int i = 0; i < windowsToProcess.Count; i++ ) {
                rData.Update.Invoke( windowsToProcess[i], null );
            }
        }

        private void PopupGUI( int id ) {
            if ( popup != null ) {
                pData.GUI.Invoke( popup, null );
                GUI.BringWindowToFront( id );
                GUI.FocusWindow( id );
            }
        }

        private void WindowGUI( int id ) {
            var wnd = windows.Where( w => w.WindowID == id ).FirstOrDefault();
            if ( wnd != null ) {
                rData.GUI.Invoke( wnd, null );
            }
        }

        public void ShowPopup( ExtendedPopup popup ) {
            RemovePopup();

            popup.Editor = this;
            pData.Initialize.Invoke( popup, new object[] { GenerateID() } );
            this.popup = popup;

            Repaint();
        }

        public void RemovePopup() {
            if ( popup == null ) return;

            pData.Destroy.Invoke( popup, null );
            popup = null;

            Repaint();
        }

        public void AddWindow( ExtendedWindow window ) {
            window.Editor = this;

            rData.Initialize.Invoke( window, new object[] { GenerateID() } );
            windows.Add( window );

            Repaint();
        }

        public void RemoveWindow( ExtendedWindow window ) {
            rData.Destroy.Invoke( window, null );
            windows.Remove( window );

            Repaint();
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

            editorWindow.windowsToAdd = new List<ExtendedWindow>( windows );

            return editorWindow;
        }

        public static ExtendedEditor CreateEditor( string title, params ExtendedWindow[] windows ) {
            var inst = CreateEditor( windows );

            inst.titleContent = new GUIContent( title );
            inst.Assets.Initialize();

            var index = 0;
            var id = string.Format( "tnrd_editor_{0}_{1}", title, index );
            while ( EditorPrefs.HasKey( id ) ) {
                index++;
                id = string.Format( "tnrd_editor_{0}_{1}", title, index );
            }

            inst.name = id;

            return inst;
        }

        public void OnBeforeSerialize() {
            var sEditor = new SerializableEditor();
            sEditor.IsInitialized = isInitialized;
            sEditor.IsInitializedGUI = isInitializedGUI;
            sEditor.WindowIDs = windowIDs;
            sEditor.Windows = windows;

            var b64 = Serializer.SerializeToB64( sEditor );
            EditorPrefs.SetString( name, b64 );
        }

        public void OnAfterDeserialize() {
            if ( EditorPrefs.HasKey( name ) ) {
                var b64 = EditorPrefs.GetString( name );
                EditorPrefs.DeleteKey( name );

                var sEditor = Deserializer.Deserialize<SerializableEditor>( b64 );
                isInitialized = sEditor.IsInitialized;
                isInitializedGUI = sEditor.IsInitializedGUI;
                windowIDs = sEditor.WindowIDs;
                windows = sEditor.Windows;

                OnDeserialized();
            }
        }

        private int GenerateID() {
            windowIDs++;
            return windowIDs;
        }

        private struct SerializableEditor {
            public bool IsInitialized;
            public bool IsInitializedGUI;
            public int WindowIDs;
            public List<ExtendedWindow> Windows;
        }
    }
}